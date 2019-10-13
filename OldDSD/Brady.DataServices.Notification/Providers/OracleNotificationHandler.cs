using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

namespace Brady.DataServices.Notification
{
    public class OracleNotificationHandler : NotificationHandler<OracleNotificationProvider>
    {
        readonly string _surrogateKey = "rowid";

        OracleDependency _dependency;
        string _querySql;
        Dictionary<Key, KeyAndRowHash> _dataIndex;
        
        public OracleNotificationHandler()
            : base()
        { }

        public OracleNotificationHandler(OracleNotificationProvider provider, INotificationQuery query)
            : this()
        {
            Initialise(provider, query);   
        }

        public override void Initialise(INotificationProvider provider, INotificationQuery query)
        {
            base.Initialise(provider, query);
                        
            Query.Initialise(new OracleConnection(Provider.ConnectionString));

            if (query is NotificationTable && query.NotificationMechanism == NotificationMechanism.Dependency)
                _querySql = ((NotificationTable)query).GetTableSql(NotificationTable.DefaultTableAlias, _surrogateKey);
            else
                _querySql = query.Sql;

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Initialising handler for query '{0}': {1}", query.Name, _querySql);
        }
               

        public override IDisposable OnSubscribe(IObserver<Notification> observer)
        {
            if (observer != null)
            {
                if (Observers == null || Observers.Count == 0)
                {
                    switch(Query.NotificationMechanism)
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

            base.OnFinalUnsubscribe();
        }

        private void RegisterDependency()
        {
            try
            {
                Dictionary<Key, KeyAndRowHash> newDataIndex = new Dictionary<Key, KeyAndRowHash>();

                using (OracleConnection connection = new OracleConnection(Provider.ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand(_querySql, connection))
                    {
                        _dependency = new OracleDependency(command, false, 0, false);
                        _dependency.OnChange += dependency_OnChange;

                        connection.Open();
                        try
                        {
                            using (OracleDataReader reader = command.ExecuteReader())
                            {
                                if (Query.NotificationLevel == NotificationLevel.Row || 
                                    Query.NotificationMechanism == NotificationMechanism.Poll)
                                {
                                    while (reader.Read())
                                    {
                                        Key surrogateKey = reader.GetKey(_surrogateKey);
                                        Key key = reader.GetKey(Query.KeyFieldNames);
                                        int rowHash = reader.GetRow().GetHashCode();
                                        newDataIndex.Add(surrogateKey, new KeyAndRowHash(key, rowHash));
                                    }
                                }
                                reader.Close();
                            }
                        }
                        catch (OracleException ex)
                        {
                            _querySql = Query.Sql;
                            if (ex.Number == 1446) //cannot include rowid in sql statement
                            {
                                if (_log.IsWarnEnabled)
                                    _log.WarnFormat("Failed to register database dependency for query '{0}' - {1}", _querySql, ex.Message);

                                RegisterDependency();
                            }
                            else
                            {
                                if (_log.IsWarnEnabled)
                                    _log.WarnFormat("Falling back to polling for query '{0}', sql = {1}", Query.Name, _querySql);

                                //Fallback on polling
                                Provider.BeginPoll(this);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (_log.IsErrorEnabled)
                                _log.ErrorFormat("Failed to register database dependency for query '{0}': {1}", Query.Name, ex.Message);
                            throw ex;
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
                _dataIndex = newDataIndex;
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.ErrorFormat("Failed to register database dependency for query '{0}' - {1}", _querySql, ex.Message);
                throw ex;
            }
        }
        
        void UpdateNotifications()
        {
            try
            {
                if (Query.NotificationLevel == NotificationLevel.Interval)
                    Notifications.Add(new Notification(NotificationType.Interval, Query.Name, null, null));
                else
                {
                    Dictionary<Key, KeyAndRowHash> newDataIndex = new Dictionary<Key, KeyAndRowHash>();

                    using (OracleConnection connection = new OracleConnection(Provider.ConnectionString))
                    using (OracleCommand command = new OracleCommand(_querySql, connection))
                    {
                        connection.Open();
                        using (OracleDataReader reader = command.ExecuteReader())
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
                        if (Query.NotificationLevel == NotificationLevel.Row)
                        {
                            foreach (var item in _dataIndex)
                            {
                                if (newDataIndex.ContainsKey(item.Key))
                                {
                                    if (newDataIndex[item.Key].RowHash != item.Value.RowHash) //updated item
                                        Notifications.Add(new Notification(NotificationType.Update, Query.Name, Query.KeyFieldNames, item.Key.Values));
                                }
                                else //deleted item
                                    Notifications.Add(new Notification(NotificationType.Delete, Query.Name, Query.KeyFieldNames, item.Key.Values));
                            }
                            foreach (var item in newDataIndex)
                            {
                                if (!_dataIndex.ContainsKey(item.Key)) //inserted items
                                    Notifications.Add(new Notification(NotificationType.Insert, Query.Name, Query.KeyFieldNames, item.Key.Values));
                            }
                        }
                        else
                            Notifications.Add(new Notification(NotificationType.Table, Query.Name, null, null));
                    }

                    _dataIndex = newDataIndex;
                }
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.ErrorFormat("Failed to update notifications for query '{0}' - {1}", _querySql, ex.Message);
                throw ex;
            }
        }

        public override void Poller()
        {
            UpdateNotifications();
            RaiseNotifications();
        }

        void dependency_OnChange(object sender, OracleNotificationEventArgs eventArgs)
        {
            try
            {
                if (Observers.Count != 0)
                {
                    if (Notifications != null)
                    {
                        foreach (System.Data.DataRow detailsRow in eventArgs.Details.Rows)
                        {
                            string rowId = detailsRow["rowid"].ToString();

                            if (string.IsNullOrEmpty(rowId))
                            {
                                UpdateNotifications();
                                break;
                            }
                            else
                            {
                                string resourceName = detailsRow["resourcename"].ToString().ToLower();
                                OracleNotificationInfo oracleNotificationInfo = (OracleNotificationInfo)detailsRow["info"];
                                NotificationType notificationType = NotificationType.None;
                                
                                if ((oracleNotificationInfo & OracleNotificationInfo.Insert) == OracleNotificationInfo.Insert)
                                    notificationType = NotificationType.Insert;
                                else if ((oracleNotificationInfo & OracleNotificationInfo.Update) == OracleNotificationInfo.Update)
                                    notificationType = NotificationType.Update;
                                else if ((oracleNotificationInfo & OracleNotificationInfo.Delete) == OracleNotificationInfo.Delete)
                                    notificationType = NotificationType.Delete;
                                else
                                    notificationType = NotificationType.Table;

                                if (notificationType != NotificationType.None)
                                {
                                    if (Query.NotificationLevel == NotificationLevel.Row ||
                                        Query.NotificationMechanism == NotificationMechanism.Poll)
                                    {
                                        Key surrogateKey = new Key(rowId);
                                        Key key;
                                        if (notificationType == NotificationType.Delete)
                                        {
                                            key = _dataIndex[surrogateKey].Key;
                                            _dataIndex.Remove(surrogateKey);
                                        }
                                        else
                                        {
                                            UpdateDataIndex(surrogateKey, notificationType);
                                            key = _dataIndex[surrogateKey].Key;
                                        }
                                        Notifications.Add(new Notification(notificationType, Query.Name, Query.KeyFieldNames, key.Values));
                                    }
                                    else
                                    {
                                        Notifications.Add(new Notification(notificationType, Query.Name, null, null));
                                    }
                                }
                            }
                        }
                        RaiseNotifications();
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.ErrorFormat("Failed to process database notification for query '{0}' - {1}", Query.Name, ex.Message);
            }
        }

        private void UpdateDataIndex(Key indexKey, NotificationType notificationType)
        {
            try
            {
                if (notificationType == NotificationType.Delete)
                    _dataIndex.Remove(indexKey);
                else
                {
                    string rowId = (string)indexKey[0];
                    string rowSql = _querySql + " WHERE rowid = :rid";

                    using (OracleConnection connection = new OracleConnection(Provider.ConnectionString))
                    using (OracleCommand command = new OracleCommand(rowSql, connection))
                    {
                        connection.Open();
                        command.Parameters.Add("rid", OracleDbType.Varchar2, rowId.Length, (object)rowId, System.Data.ParameterDirection.Input);

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Key key = reader.GetKey(Query.KeyFieldNames);
                                int rowHash = reader.GetRow().GetHashCode();

                                if (notificationType == NotificationType.Insert)
                                    _dataIndex.Add(indexKey, new KeyAndRowHash(key, rowHash));
                                else
                                    _dataIndex[indexKey] = new KeyAndRowHash(key, rowHash);
                            }
                            reader.Close();
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.ErrorFormat("Failed to update data index for query '{0}' - {1}", Query.Name, ex.Message);
                throw ex;
            }
        }
    }
}
