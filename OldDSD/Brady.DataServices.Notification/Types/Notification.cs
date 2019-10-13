using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public enum NotificationType
    { 
        None,
        Insert,
        Update,
        Delete,
        Table,
        Interval
    }

    public class Notification
    {
        public Notification(NotificationType type, string queryName, string[] keyFieldNames = null, object[] keyFieldValues = null)
        {
            Type = type;
            QueryName = queryName;
            KeyFieldNames = keyFieldNames;
            KeyFieldValues = keyFieldValues;
        }

        public NotificationType Type { get; set; }
        public string QueryName { get; set; }
        public string[] KeyFieldNames { get; set; }
        public object[] KeyFieldValues { get; set; }
    }
}
