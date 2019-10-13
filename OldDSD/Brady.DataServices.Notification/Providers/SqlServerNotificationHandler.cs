using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public class SqlServerNotificationHandler : NotificationHandler<SqlServerNotificationProvider>
    {
        readonly ILog log = LogManager.GetLogger(typeof(SqlServerNotificationHandler));
        SqlDependency _dependency;
        Dictionary<Key, KeyAndRowHash> _dataIndex;

        public SqlServerNotificationHandler()
            : base()
        {
        }

        public SqlServerNotificationHandler(SqlServerNotificationProvider provider, INotificationQuery query)
            : this()
        {
            base.Initialise(provider, query);

            Query.Initialise(new SqlConnection(Provider.ConnectionString));
        }

        public override IDisposable OnSubscribe(IObserver<Notification> observer)
        {
            if (observer != null)
            {
                if (Observers == null || Observers.Count == 0)
                {
                    switch (Query.NotificationMechanism)
                    {
                        case NotificationMechanism.Dependency:
                            RegisterDependency();
                            break;
                        case NotificationMechanism.Poll:
                            Provider.BeginPoll(this);
                            break;
                    }
                }
                return base.OnSubscribe(observer);
            }
            else
                throw new ArgumentException("Observer required");
        }

        public override void OnFinalUnsubscribe()
        {
            if (_dependency != null)
                _dependency.OnChange -= dependency_OnChange;

            Provider.EndPoll(this);

            SqlDependency.Stop(Provider.ConnectionString);

            base.OnFinalUnsubscribe();
        }

        private void RegisterDependency()
        {
            try
            {
                Dictionary<Key, KeyAndRowHash> newDataIndex = new Dictionary<Key, KeyAndRowHash>();

                using (SqlConnection connection = new SqlConnection(Provider.ConnectionString))
                using (SqlCommand command = new SqlCommand(Query.Sql, connection))
                {
                    _dependency = new SqlDependency(command);
                    _dependency.OnChange += dependency_OnChange;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (Query.NotificationLevel == NotificationLevel.Row ||
                            Query.NotificationMechanism == NotificationMechanism.Poll)
                        {
                            while (reader.Read())
                            {
                                Key key = reader.GetKey(Query.KeyFieldNames);
                                int rowHash = reader.GetRow().GetHashCode();
                                newDataIndex.Add(key, new KeyAndRowHash(key, rowHash));
                            }
                        }
                        reader.Close();
                    }
                    connection.Close();
                }

                if (_dataIndex != null)
                {
                    if (Query.NotificationLevel == NotificationLevel.Table)
                    {
                        Notifications.Add(new Notification(NotificationType.Table, Query.Name, null, null));
                    }
                    else
                    {
                        foreach (var item in _dataIndex)
                        {
                            if (newDataIndex.ContainsKey(item.Key))
                            {
                                if (newDataIndex[item.Key].RowHash != item.Value.RowHash) //updated item
                                {
                                    Notifications.Add(new Notification(NotificationType.Update, Query.Name, Query.KeyFieldNames, item.Key.Values));
                                }
                            }
                            else //deleted item
                            {
                                Notifications.Add(new Notification(NotificationType.Delete, Query.Name, Query.KeyFieldNames, item.Key.Values));
                            }
                        }
                        foreach (var item in newDataIndex)
                        {
                            if (!_dataIndex.ContainsKey(item.Key)) //inserted items
                            {
                                Notifications.Add(new Notification(NotificationType.Insert, Query.Name, Query.KeyFieldNames, item.Key.Values));
                            }
                        }
                    }
                }
                _dataIndex = newDataIndex;
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.ErrorFormat("Failed to register database dependency for query '{0}' - {1}", Query.Sql, ex.Message);
                throw ex;
            }
        }

        public override void Poller()
        {
            try
            {
                if (Query.NotificationLevel == NotificationLevel.Interval)
                    Notifications.Add(new Notification(NotificationType.Interval, Query.Name, null, null));
                else
                {
                    Dictionary<Key, KeyAndRowHash> newDataIndex = new Dictionary<Key, KeyAndRowHash>();

                    using (SqlConnection connection = new SqlConnection(Provider.ConnectionString))
                    using (SqlCommand command = new SqlCommand(Query.Sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Key key = reader.GetKey(Query.KeyFieldNames);
                                int rowHash = reader.GetRow().GetHashCode();
                                newDataIndex.Add(key, new KeyAndRowHash(key, rowHash));
                            }
                            reader.Close();
                        }
                        connection.Close();
                    }

                    if (_dataIndex != null)
                    {
                        foreach (var item in _dataIndex)
                        {
                            if (newDataIndex.ContainsKey(item.Key))
                            {
                                if (newDataIndex[item.Key].RowHash != item.Value.RowHash) //updated item
                                {
                                    if (Query.NotificationLevel == NotificationLevel.Table)
                                    {
                                        Notifications.Add(new Notification(NotificationType.Table, Query.Name, null, null));
                                        break;
                                    }
                                    else
                                        Notifications.Add(new Notification(NotificationType.Update, Query.Name, Query.KeyFieldNames, item.Key.Values));
                                }
                            }
                            else //deleted item
                            {
                                if (Query.NotificationLevel == NotificationLevel.Table)
                                {
                                    Notifications.Add(new Notification(NotificationType.Table, Query.Name, null, null));
                                    break;
                                }
                                else
                                    Notifications.Add(new Notification(NotificationType.Delete, Query.Name, Query.KeyFieldNames, item.Key.Values));
                            }
                        }
                        foreach (var item in newDataIndex)
                        {
                            if (!_dataIndex.ContainsKey(item.Key)) //inserted items
                            {
                                if (Query.NotificationLevel == NotificationLevel.Table)
                                {
                                    Notifications.Add(new Notification(NotificationType.Table, Query.Name, null, null));
                                    break;
                                }
                                else
                                    Notifications.Add(new Notification(NotificationType.Insert, Query.Name, Query.KeyFieldNames, item.Key.Values));
                            }
                        }
                    }
                    RaiseNotifications();

                    _dataIndex = newDataIndex;
                }
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.ErrorFormat("Polling failed for query '{0}' - {1}", Query.Sql, ex.Message);
            }
        }

        void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            SqlDependency dependency = sender as SqlDependency;
            dependency.OnChange -= dependency_OnChange;

            if (e.Type == SqlNotificationType.Subscribe &&
                e.Source == SqlNotificationSource.Statement &&
                e.Info == SqlNotificationInfo.Invalid)
            {
                if (_log.IsWarnEnabled)
                    _log.WarnFormat("Falling back to polling for query '{0}', sql = {1}", Query.Name, Query.Sql);

                //Fallback on polling
                Provider.BeginPoll(this);
            }
            else
            {
                if (Observers.Count != 0)
                {
                    RegisterDependency();
                    RaiseNotifications();
                }
            }
        }
    }
}
