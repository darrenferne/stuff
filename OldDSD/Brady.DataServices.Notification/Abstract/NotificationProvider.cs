using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using log4net;
using System.Collections.Concurrent;

namespace Brady.DataServices.Notification
{
    public abstract class NotificationProvider<T_Handler> : INotificationProvider where T_Handler : INotificationHandler, new()
    {
        protected ILog _log = LogManager.GetLogger(typeof(NotificationProvider<T_Handler>));

        Dictionary<string, T_Handler> _queryHandlers;
        Timer _pollingTimer;
        int _timerPulseRate = 100;

        internal object SynchLock = new Object();

        internal ConcurrentDictionary<NotificationRequest, RequestObserver> RequestHandlers { get; private set; }

        internal List<T_Handler> PolledHandlers { get; private set; }

        public NotificationProvider()
        { }

        public NotificationProvider(string connectionString)
        {
            ConnectionString = connectionString;
            RequestHandlers = new ConcurrentDictionary<NotificationRequest, RequestObserver>();
            _queryHandlers = new Dictionary<string, T_Handler>();
            PolledHandlers = new List<T_Handler>();
        }

        public string ConnectionString { get; set; }

        public IDisposable RegisterNotification(NotificationRequest request, IObserver<NotificationResponse> observer)
        {
            try
            {
                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("Registering notification request: {0}", request.Id);
                    foreach (var query in request.Queries)
                        _log.DebugFormat("Notification query: {0}, {1}", query.Name, query.Sql);
                }


                lock (SynchLock)
                {
                    if (RequestHandlers.Count == 0)
                        OnFirstSubscription();
                    var requestObserver = new RequestObserver(observer);
                    RequestHandlers.AddOrUpdate(request, requestObserver, (k,v) => requestObserver);

                    Parallel.ForEach(request.Queries, query =>
                    {
                        if (!_queryHandlers.ContainsKey(query.Name))
                        {
                            try
                            {
                                lock (_queryHandlers)
                                {
                                    T_Handler handler = new T_Handler();
                                    handler.Initialise(this, query);
                                    _queryHandlers.Add(query.Name, handler);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (_log.IsErrorEnabled)
                                    _log.ErrorFormat("Failed to register notification query: {0} for request: {1} - {2}", query.Name, request.Id, ex.Message);
                            }
                        }
                        var queryHandler = _queryHandlers[query.Name];
                        var subscription = queryHandler.Subscribe(Observer.Create<Notification>(OnQueryNotification));
                        request.Subscriptions.Add(subscription);
                    });
                }

                return new Unsubscriber(this, request);
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.ErrorFormat("Failed to register notification request: {0} - {1}", request.Id, ex.Message);
                throw ex;
            }
        }

        private void OnQueryNotification(Notification notification)
        {
            lock (SynchLock)
            {
                Parallel.ForEach(RequestHandlers.Keys, request =>
                {
                    if (request.Queries.Count(q => q.Name == notification.QueryName) != 0)
                    {
                        try
                        {
                            RequestObserver handler = RequestHandlers[request];
                            if (handler != null && handler.Observer != null)
                            {
                                lock (handler)
                                {
                                    handler.Observer.OnNext(new NotificationResponse(request.Id, handler.Notifications));
                                    handler.Notifications.Clear();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (_log.IsErrorEnabled)
                                _log.ErrorFormat("Notification failed for request '{0}': {1}", request.Id, ex.Message);
                        }
                    }
                });
            }
        }

        internal void BeginPoll(T_Handler handler)
        {
            PolledHandlers.Add(handler);

            StartPolling();

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Polling started for query '{0}': {1}", handler.Query.Name, handler.Query.Sql);
        }

        internal void EndPoll(T_Handler handler)
        {
            if (PolledHandlers.Contains(handler))
                PolledHandlers.Remove(handler);

            StopPolling(false);
        }

        private void StartPolling()
        {
            if (_pollingTimer == null)
            {
                _pollingTimer = new Timer(_timerPulseRate);
                _pollingTimer.Elapsed += _pollingTimer_Elapsed;
            }

            if (!_pollingTimer.Enabled)
            {
                _pollingTimer.AutoReset = true;
                _pollingTimer.Enabled = true;
                _pollingTimer.Start();

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Starting poller");
            }
        }

        private void StopPolling(bool force)
        {
            if ((PolledHandlers.Count == 0 || force) && _pollingTimer != null)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Stopping poller");

                _pollingTimer.Stop();
                _pollingTimer.Dispose();
                _pollingTimer = null;
            }
        }

        void _pollingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Parallel.ForEach(PolledHandlers, handler =>
            {
                handler.PollCounter += _timerPulseRate;
                if (!handler.IsPolling && (handler.PollCounter >= handler.Query.PollInterval))
                {
                    try
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Poll for query '{0}'", handler.Query.Name);

                        handler.IsPolling = true;
                        handler.Poller();

                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Poll complete for query '{0}'", handler.Query.Name);
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.ErrorFormat("Polling failed for query '{0}': {1}", handler.Query.Name, ex.Message);
                    }
                    finally
                    {
                        handler.IsPolling = false;
                    }
                }
                if (handler.PollCounter >= handler.Query.PollInterval)
                    handler.PollCounter = 0;

            });
        }

        internal virtual void OnFirstSubscription()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("First subscription");
        }

        internal virtual void OnFinalUnsubscribe()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Final unsubscribe");

            StopPolling(true);
            lock (SynchLock)
            {
                if (RequestHandlers.Count > 0)
                    RequestHandlers.Clear();
            }
        }

        internal class RequestObserver
        {
            DateTime LastResponse { get; set; }
            public IObserver<NotificationResponse> Observer { get; private set; }
            public List<Notification> Notifications { get; private set; }

            public RequestObserver(IObserver<NotificationResponse> observer)
            {
                LastResponse = DateTime.UtcNow;
                Notifications = new List<Notification>();
                Observer = observer;
            }
        }

        protected class Unsubscriber : IDisposable
        {
            private NotificationRequest Request { get; set; }
            private NotificationProvider<T_Handler> Provider { get; set; }

            public Unsubscriber(NotificationProvider<T_Handler> provider, NotificationRequest request)
            {
                this.Request = request;
                this.Provider = provider;
            }

            public void Dispose()
            {
                if (this.Provider.RequestHandlers != null && this.Provider.RequestHandlers.ContainsKey(this.Request))
                {
                    if (Provider._log.IsDebugEnabled)
                        Provider._log.DebugFormat("Killing request notification: {0}", Request.Id);

                    foreach (IDisposable subscription in Request.Subscriptions)
                        subscription.Dispose();

                    lock (this.Provider.SynchLock)
                    {
                        RequestObserver requestObserver;
                        this.Provider.RequestHandlers.TryRemove(this.Request, out requestObserver);
                    }

                    if (this.Provider.RequestHandlers.Count == 0)
                        this.Provider.OnFinalUnsubscribe();
                }
            }
        }
    }
}
