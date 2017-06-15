using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Brady.DataServices.Notification
{
    public abstract class NotificationHandler<T_Provider> : INotificationHandler where T_Provider : INotificationProvider
    {
        protected ILog _log = LogManager.GetLogger(typeof(NotificationHandler<T_Provider>));

        T_Provider _provider;
        INotificationQuery _query;
        List<Notification> _notifications;
        int _pollCounter;
        bool _isPolling;

        internal object SynchLock = new Object();

        public NotificationHandler()
        {
            if (_log.IsDebugEnabled)
                _log.Debug("Initialising notification handler");
            Observers = new List<IObserver<Notification>>();
            _notifications = new List<Notification>();
        }

        public T_Provider Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }

        public INotificationQuery Query
        {
            get { return _query; }
            set { _query = value; }
        }

        public virtual void Initialise(INotificationProvider provider, INotificationQuery query)
        {
            Provider = (T_Provider)provider;
            Query = query;

            NotificationTable table = query as NotificationTable;
            if (table != null)
            {
                if (table.KeyFieldNames == null || table.KeyFieldNames.Length == 0)
                    throw new ArgumentException("Invalid notification table. No key columns defined");
            }
        }

        public List<Notification> Notifications
        {
            get { return _notifications; }
        }

        public void RaiseNotifications()
        {
            if (Notifications != null)
            {
                lock (SynchLock)
                {
                    foreach (Notification notification in Notifications)
                    {
                        Parallel.ForEach(Observers, observer =>
                        {
                            try
                            {
                                observer.OnNext(notification);
                            }
                            catch (Exception ex)
                            {
                                if (_log.IsErrorEnabled)
                                    _log.ErrorFormat("Notification failed for query '{0}': {1}", Query.Name, ex.Message);
                            }
                        });
                    }
                    Notifications.Clear();
                }
            }
        }

        public List<IObserver<Notification>> Observers { get; private set; }

        public virtual IDisposable OnSubscribe(IObserver<Notification> observer)
        {
            if (!Observers.Contains(observer))
            {
                lock (SynchLock)
                {
                    Observers.Add(observer);
                }
            }
            return new Unsubscriber(this, observer);
        }

        public virtual void OnFinalUnsubscribe()
        {
            if (Observers.Count > 0)
                Observers.Clear();
        }

        public int PollCounter
        {
            get { return _pollCounter; }
            set
            {
                //lock(SynchLock)
                _pollCounter = value;
            }
        }

        public bool IsPolling
        {
            get { return _isPolling; }
            set
            {
                lock (SynchLock)
                    _isPolling = value;
            }
        }

        public abstract void Poller();

        public IDisposable Subscribe(IObserver<Notification> observer)
        {
            return this.OnSubscribe(observer);
        }

        protected class Unsubscriber : IDisposable
        {
            private NotificationHandler<T_Provider> Handler { get; set; }
            private IObserver<Notification> Observer { get; set; }

            public Unsubscriber(NotificationHandler<T_Provider> handler, IObserver<Notification> observer)
            {
                this.Handler = handler;
                this.Observer = observer;
            }

            public void Dispose()
            {
                if (this.Handler.Observers != null && this.Handler.Observers.Contains(this.Observer))
                {
                    if (Handler._log.IsDebugEnabled)
                        Handler._log.DebugFormat("Killing query notification: {0}", Handler.Query.Name);

                    lock (Handler.SynchLock)
                    {
                        this.Handler.Observers.Remove(this.Observer);
                        if (this.Handler.Observers.Count == 0)
                            this.Handler.OnFinalUnsubscribe();
                    }
                }

            }
        }
    }
}
