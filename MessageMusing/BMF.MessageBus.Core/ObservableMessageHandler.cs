using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core
{
    public class ObservableMessageHandler<T_message> : MessageHandler<T_message>
    {
        private Subject<T_message> _messageSubject = new Subject<T_message>();

        public IObservable<T_message> Messages
        {
            get
            {
                return _messageSubject.AsObservable();
            }
        }

        public override void HandleMessage(T_message message)
        {
            _messageSubject.OnNext(message);
        }
    }
}
