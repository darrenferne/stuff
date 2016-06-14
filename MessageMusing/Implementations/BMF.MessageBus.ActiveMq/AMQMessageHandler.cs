using Apache.NMS;
using Apache.NMS.Util;
using BMF.MessageBus.Core;
using BMF.MessageBus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.ActiveMq
{
    public class AMQMessageHandler<T_message, T_handler> : AMQMessageHandler where T_handler : MessageHandler<T_message>, new()
    {
        public AMQMessageHandler()
        { }

        public override void HandleMessage(IMessage aqMsg)
        {
            byte[] body = ((IBytesMessage)aqMsg).Content;
            T_message message = _bus._serialiser.Deserialise<T_message>(body);

            var handler = _container.ResolveType<T_handler>();
            handler.Bus = _bus;
            handler.HandleMessage(message);
        }
    }

    public abstract class AMQMessageHandler
    {
        protected IMessageBusContainer _container;
        protected AMQMessageBus _bus;
        protected IMessageConsumer _consumer;

        public void Consume(AMQMessageBus bus, string queueName)
        {
            _bus = bus;

            var destination = _bus._session.GetDestination(queueName);

            _consumer = _bus._session.CreateConsumer(destination);
            _consumer.Listener += HandleMessage;
        }

        public void Subscribe(AMQMessageBus bus, string endpointName, string topicName)
        {
            _bus = bus;

            var selector = "2 > 1"; //SQL 92 query syntax applied to msg header
            var topic = _bus._session.GetTopic(topicName);

            _consumer = bus._session.CreateDurableConsumer(topic, endpointName, selector, false);
            _consumer.Listener += HandleMessage;
        }

        public abstract void HandleMessage(IMessage aqMsg);

        public static AMQMessageHandler Create(IMessageBusContainer container, MessageMetadata metadata)
        {
            var genericHandlerType = typeof(AMQMessageHandler<,>);
            var specificHandlerType = genericHandlerType.MakeGenericType(metadata.MessageType, metadata.HandlerType);

            var handler = (AMQMessageHandler)Activator.CreateInstance(specificHandlerType);
            handler._container = container;

            return handler;
        }
    }
}
