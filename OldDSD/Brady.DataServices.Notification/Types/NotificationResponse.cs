using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public class NotificationResponse
    {
        public NotificationResponse()
        { }

        public NotificationResponse(string requestId, Notification notification)
            : this(requestId, new Notification[] { notification })
        { }

        public NotificationResponse(string requestId, IEnumerable<Notification> notifications)
        {
            RequestId = requestId;
            Notifications = notifications.ToArray();
        }

        public string RequestId { get; set; }
        public Notification[] Notifications { get; set;} 
    }
}
