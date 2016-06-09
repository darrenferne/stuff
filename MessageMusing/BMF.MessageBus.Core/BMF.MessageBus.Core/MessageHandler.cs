using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core
{
    public class MessageHandler<T_message> : IMessageHandler<T_message>
    {
        private Subject<T_message> _messageSubject = new Subject<T_message>();
                        
        public IObservable<T_message> Messages
        {
            get
            {
                return _messageSubject.AsObservable();
            }
        }

        public virtual void HandleMessage(T_message message)
        {
            _messageSubject.OnNext(message);
        }
    }
}
