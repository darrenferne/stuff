using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Brady.DataServices.Notification
{
    public class NotificationManager : IDisposable
    {
        ILog _log = LogManager.GetLogger(typeof(NotificationManager));
        NotificationConfiguration _config;

        string _databaseType;
        string _databaseName;
        string _connectionString;
        NotificationMechanism _defaultNotificationMechanism;
        NotificationLevel _defaultNotificationLevel;
        int _defaultPollInterval;

        INotificationProvider _provider;
        List<IDisposable> _preLoadedRequests;

        bool _disposed;

        public NotificationManager(string databaseType, string databaseName, string connectionString = "")
        {
            _databaseType = databaseType;
            _databaseName = databaseName;
            
            try
            {
                _config = NotificationConfiguration.Current;
                if (string.IsNullOrEmpty(connectionString))
                {
                    _connectionString = _config.Databases.GetConnectionString(databaseType, databaseName);
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        var element = ConfigurationManager.ConnectionStrings[databaseName];
                        if (element != null)
                            _connectionString = element.ConnectionString;
                    }
                }
                else
                    _connectionString = connectionString;

                _defaultNotificationMechanism = _config.DefaultNotificationMechanism;
                _defaultNotificationLevel = _config.DefaultNotificationLevel;
                _defaultPollInterval = _config.DefaultPollInterval;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to read notification configuration", ex);
            }

            if (string.IsNullOrEmpty(_connectionString))
                throw new ArgumentException("Connection string required");
            
            LoadProvider(_databaseType);
            PreLoadTables();
        }

        private void LoadProvider(string databaseType)
        {
            try
            {
                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("Loading notification provider for database type {0}", _databaseType);
                    _log.DebugFormat("Database Name: {0}", _databaseName);
                    _log.DebugFormat("Connection String: {0}", _connectionString);
                    _log.DebugFormat("Default Notification Mechanism: {0}", _defaultNotificationMechanism);
                    _log.DebugFormat("Default Poll Interval: {0}", _defaultPollInterval);
                }

                switch (databaseType.ToLowerInvariant())
                {
                    case "sqlserver":
                        _provider = new SqlServerNotificationProvider(_connectionString);
                        break;
                    case "oracle":
                        _provider = new OracleNotificationProvider(_connectionString);
                        break;
                    default:
                        if (_log.IsErrorEnabled)
                            _log.ErrorFormat("Notification Provider not available for database type: {0}", databaseType);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.ErrorFormat("Failed to load notification provider for database type '{0}': {1}", _databaseType, ex.Message);
                throw ex;
            }
        }
        
        private void ApplyOverrides(NotificationRequest request)
        {
            try
            {
                List<INotificationQuery> newQueries = new List<INotificationQuery>();
                foreach (INotificationQuery query in request.Queries)
                {
                    NotificationTable table = query as NotificationTable;
                    if (table == null)
                        newQueries.Add(query);
                    else
                    {
                        var tableOverride = _config.TableOverrides.GetTable(table.Name, _databaseType, _databaseName);
                        if (tableOverride == null)
                            newQueries.Add(query);
                        else
                        {
                            if (tableOverride.Overrides.Count == 0)
                            {
                                table.PollInterval = tableOverride.PollInterval == 0 ? table.PollInterval : tableOverride.PollInterval;
                                table.NotificationMechanism = tableOverride.NotificationMechanism == NotificationMechanism.None ? table.NotificationMechanism : tableOverride.NotificationMechanism;
                                table.NotificationLevel = tableOverride.NotificationLevel == NotificationLevel.None ? table.NotificationLevel : tableOverride.NotificationLevel;
                                newQueries.Add(table);

                                if (_log.IsInfoEnabled)
                                    _log.InfoFormat("Notification Table override: {0}, Poll Interval={1}, Poll Mechanism={2}", table.Name, table.PollInterval, table.NotificationMechanism);
                            }
                            else
                            {
                                foreach (TableElement newTable in tableOverride.Overrides)
                                {
                                    string newTableName = newTable.Table;
                                    string[] newTableKey = null;
                                    if (!string.IsNullOrEmpty(newTable.Key))
                                        newTableKey = newTable.Key.Split(',');

                                    NotificationMechanism newNotificationMechanism = newTable.NotificationMechanism == NotificationMechanism.None ? tableOverride.NotificationMechanism : newTable.NotificationMechanism;
                                    newNotificationMechanism = newNotificationMechanism == NotificationMechanism.None ? table.NotificationMechanism : newNotificationMechanism;

                                    NotificationLevel newNotificationLevel = newTable.NotificationLevel == NotificationLevel.None ? tableOverride.NotificationLevel : newTable.NotificationLevel;
                                    newNotificationLevel = newNotificationLevel == NotificationLevel.None ? table.NotificationLevel : newNotificationLevel;

                                    int newPollInterval = newTable.PollInterval == 0 ? tableOverride.PollInterval : newTable.PollInterval;
                                    newPollInterval = newPollInterval == 0 ? table.PollInterval : newPollInterval;
                                    
                                    newQueries.Add(new NotificationTable(newTableName, newNotificationMechanism, newNotificationLevel, newPollInterval, newTableName, newTableKey));

                                    if (_log.IsInfoEnabled)
                                        _log.InfoFormat("Notification Table override: {0}, New Table={1}, New Table Key='{2}', Poll Interval={3}, Notification Mechanism={4}, Notification Level={4}", table.Name, newTableName, newTable.Key, newPollInterval, newNotificationMechanism, newNotificationLevel);
                                }
                            }
                        }
                    }
                }
                request.Queries = newQueries.ToArray();
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                {
                    foreach (var query in request.Queries)
                        _log.ErrorFormat("Failed to apply table overloads: {0}", query.Name, ex.Message);
                }
            }
        }

        private void PreLoadTables()
        {
            _preLoadedRequests = new List<IDisposable>();
            List<NotificationRequest> requests = new List<NotificationRequest>();

            try
            {
                foreach (TableElement table in _config.PreLoadedTables)
                {
                    if (string.IsNullOrEmpty(table.DatabaseType) || string.Compare(table.DatabaseType, _databaseType, true) == 0)
                    {
                        if (string.IsNullOrEmpty(table.DatabaseName) || string.Compare(table.DatabaseName, _databaseName, true) == 0)
                        {
                            string tableName = table.Table;
                            string[] tableKey = null;
                            if (!string.IsNullOrEmpty(table.Key))
                                tableKey = table.Key.Split(',');
                            NotificationMechanism notificationMechanism = table.NotificationMechanism == NotificationMechanism.None ? _config.DefaultNotificationMechanism : table.NotificationMechanism;
                            NotificationLevel notificationLevel = table.NotificationLevel == NotificationLevel.None ? _config.DefaultNotificationLevel : table.NotificationLevel;
                            int pollInterval = table.PollInterval == 0 ? _config.DefaultPollInterval : table.PollInterval;
                            requests.Add(new NotificationRequest(Guid.NewGuid().ToString(), new NotificationTable(tableName, notificationMechanism, notificationLevel, pollInterval, tableName, tableKey)));
                        }
                    }
                }

                Parallel.ForEach(requests, request =>
                {
                    try
                    {
                        _preLoadedRequests.Add(RegisterNotification(request, null));
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsErrorEnabled)
                        {
                            foreach (var query in request.Queries)
                                _log.ErrorFormat("Failed to pre-load table: {0} - {1}", query.Name, ex.Message);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                {
                    _log.ErrorFormat("Failed to pre-load tables: {0}", ex.Message);
                }
            }
        }

        public IDisposable RegisterNotification(NotificationRequest request, IObserver<NotificationResponse> observer)
        {
            try
            {
                ApplyOverrides(request);

                foreach (var query in request.Queries)
                {
                    if (query.NotificationMechanism == NotificationMechanism.None)
                        query.NotificationMechanism = _defaultNotificationMechanism;

                    if (query.NotificationLevel == NotificationLevel.None)
                        query.NotificationLevel = _defaultNotificationLevel;

                    if (query.PollInterval <= 0)
                        query.PollInterval = _defaultPollInterval;

                    if (query.PollInterval != 0)
                    {
                        if (request.ResponsesPerSecond == 0 || (1000 / query.PollInterval) > request.ResponsesPerSecond)
                            request.ResponsesPerSecond = (double)1000 / (double)query.PollInterval;
                    }
                }

                if (request.NotificationsPerResponse == 0)
                    request.NotificationsPerResponse = 1;

                return _provider.RegisterNotification(request, observer);
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.ErrorFormat("Failed to register notificaton request: {0}", ex.Message);
                throw ex;
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (_preLoadedRequests != null)
                {
                    foreach (IDisposable request in _preLoadedRequests)
                        request.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
