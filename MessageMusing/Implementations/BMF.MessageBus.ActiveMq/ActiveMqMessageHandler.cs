using Apache.NMS;
using Apache.NMS.Util;
using BMF.MessageBus.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.ActiveMq
{
    public class ActiveMqMessageHandler<T_message> : MessageHandler<T_message>
    {
        ActiveMqBus _bus;
        IMessageConsumer _consumer;

        public ActiveMqMessageHandler(ActiveMqBus bus, string queueName)
        {
            _bus = bus;

            var destination = SessionUtil.GetDestination(_bus._session, "queue://" + queueName);
            _consumer = bus._session.CreateConsumer(destination);
            _consumer.Listener += internalHandler; ;
        }

        private void internalHandler(IMessage aqMsg)
        {
            byte[] body = ((IBytesMessage)aqMsg).Content;

            T_message message = _bus._serialiser.Deserialise<T_message>(body);

            this.HandleMessage(message);
        }
    }
}
