using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.DataServices.Notification
{
    public interface INotificationHandler : IObservable<Notification>
    {
        INotificationQuery Query { get; }
        void Initialise(INotificationProvider provider, INotificationQuery query);
        List<IObserver<Notification>> Observers { get; }
        int PollCounter { get; set; }
        bool IsPolling { get; set; }
        void Poller();
    }
}
