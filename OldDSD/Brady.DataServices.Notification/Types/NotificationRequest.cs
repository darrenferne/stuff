using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public class NotificationRequest
    {
        List<IDisposable> _notificationSubscriptions;
   
        public NotificationRequest(string id, INotificationQuery query)
            : this(id, 0, 0, new INotificationQuery[] { query })
        { }

        public NotificationRequest(string id, INotificationQuery[] queries)
            : this(id, 0, 0, queries)
        { }

        public NotificationRequest(string id, double responsesPerSecond, int notificationsPerResponse, INotificationQuery query)
            : this(id, responsesPerSecond, notificationsPerResponse, new INotificationQuery[] { query })
        { }

        public NotificationRequest(string id, double responsesPerSecond, int notificationsPerResponse, INotificationQuery[] queries)
        {
            Id = id;
            ResponsesPerSecond = responsesPerSecond;
            NotificationsPerResponse = notificationsPerResponse;
            Queries = queries;

            _notificationSubscriptions = new List<IDisposable>();
        }

        public string Id { get; set; }
        public double ResponsesPerSecond { get; set; }
        public int NotificationsPerResponse { get; set; }
        public INotificationQuery[] Queries { get; set; }

        internal List<IDisposable> Subscriptions 
        {
            get { return _notificationSubscriptions; } 
        }
    }
}
