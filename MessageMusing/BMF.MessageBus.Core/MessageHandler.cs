using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using BMF.MessageBus.Core.Interfaces;

namespace BMF.MessageBus.Core
{
    public class MessageHandler<T_message> : IMessageHandler<T_message>
    {
        private IMessageBus _bus;

        private Type _messageType = typeof(T_message);

        public IMessageBus Bus
        {
            get { return _bus; }
            set { _bus = value; }
        }

        public Type MessageType { get { return _messageType; } }

        public virtual void HandleMessage(T_message message)
        {
        }
    }
}
