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
    public class AMQMessageHandler<T_message> : MessageHandler<T_message>
    {
        AMQMessageBus _bus;
        IMessageConsumer _consumer;

        public AMQMessageHandler(AMQMessageBus bus, string queueName)
        {
            _bus = bus;

            var durable = false;
            var selector = "2 > 1"; //SQL 92 syntax applied to msg header

            //durable pub/sub
            if (durable)
            {
                var consumerId = Guid.NewGuid().ToString();
                var topic = _bus._session.GetTopic(queueName);
                _consumer = bus._session.CreateDurableConsumer(topic, consumerId, selector, false);
            }
            else
            {
                //non durable recieve
                var destination = SessionUtil.GetDestination(_bus._session, "queue://" + queueName);
                _consumer = bus._session.CreateConsumer(destination);
            }

            _consumer.Listener += internalHandler; ;
        }

        public override void HandleMessage(T_message message)
        {
            throw new NotImplementedException();
        }

        private void internalHandler(IMessage aqMsg)
        {
            byte[] body = ((IBytesMessage)aqMsg).Content;

            T_message message = _bus._serialiser.Deserialise<T_message>(body);

            this.HandleMessage(message);
        }
    }
}
