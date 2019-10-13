using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public enum NotificationMechanism
    { 
        None,
        Dependency,
        Poll
    }

    public enum NotificationLevel
    {
        None,
        Row,
        Table,
        Interval
    }
       
    public interface INotificationQuery
    {
        string Name { get; }
        NotificationLevel NotificationLevel { get; set; }
        NotificationMechanism NotificationMechanism { get; set; }
        int PollInterval { get; set; }
        string Sql { get; set; }
        string[] KeyFieldNames { get; set; }
    }
}
