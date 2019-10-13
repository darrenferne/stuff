using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public class NotificationQuery : INotificationQuery
    {
        private string _name;

        public NotificationQuery()
        { }

        public NotificationQuery(string name, NotificationMechanism notificationMechanism, NotificationLevel notificationLevel, int pollInterval, string sql, string keyFieldName)
            : this(name, notificationMechanism, notificationLevel,pollInterval, sql, new string[] { keyFieldName })
        { }

        public NotificationQuery(string name, NotificationMechanism notificationMechanism, NotificationLevel notificationLevel, int pollInterval, string sql, string[] keyFieldNames)
        {
            Name = name;
            NotificationMechanism = notificationMechanism;
            NotificationLevel = notificationLevel;
            PollInterval = pollInterval;
            Sql = sql;
            KeyFieldNames = keyFieldNames;
        }

        public string Name
        {
            get 
            {
                if (string.IsNullOrEmpty(_name))
                    _name = Sql;
                return _name; 
            }
            set { _name = value; }
        }

        public NotificationMechanism NotificationMechanism { get; set; }
        public NotificationLevel NotificationLevel { get; set; }
        public int PollInterval { get; set; }
        public string Sql { get; set; }
        public string[] KeyFieldNames { get; set; }
    }
}
