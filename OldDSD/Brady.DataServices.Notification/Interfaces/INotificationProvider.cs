using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public interface INotificationProvider 
    {
        IDisposable RegisterNotification(NotificationRequest request, IObserver<NotificationResponse> observer);
    }
}
