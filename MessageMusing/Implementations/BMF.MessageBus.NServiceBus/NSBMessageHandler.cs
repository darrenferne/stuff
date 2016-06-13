using BMF.MessageBus.Core;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.NServiceBus
{
    public class NSBMessageHandler<T_message, T_handler> : IHandleMessages<T_message> where T_handler : MessageHandler<T_message>, new()
    {
        NSBMessageBus _bus;

        public NSBMessageBus Bus
        {
            get { return _bus; }
            set { _bus = value; }
        }

        public NSBMessageHandler()
        { }

        public NSBMessageHandler(NSBMessageBus bus, string queueName)
        {
            _bus = bus;
        }

        public void Handle(T_message message)
        {
            var handler = new T_handler();
            handler.Bus = _bus;
            handler.HandleMessage(message);
        }
    }
}
