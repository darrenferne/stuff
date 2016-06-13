using BMF.MessageBus.Core;
using BMF.MessageBus.Core.Interfaces;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.RabbitMq
{
    public class RMQMessageHandler<T_message, T_handler> : RMQMessageHandler where T_handler : MessageHandler<T_message>, new()
    {
        public RMQMessageHandler()
        { }

        public override void HandleMessage(object sender, BasicDeliverEventArgs e)
        {
            byte[] body = e.Body;
            T_message message = _bus._serialiser.Deserialise<T_message>(body);

            var handler = _container.ResolveType<T_handler>();
            handler.Bus = _bus;
            handler.HandleMessage(message);

            if (_acknowledge)
                _consumer.Model.BasicAck(e.DeliveryTag, false);
        }
    }

    public abstract class RMQMessageHandler
    {
        protected IMessageBusContainer _container;
        protected RMQMessageBus _bus;
        protected EventingBasicConsumer _consumer;
        protected bool _acknowledge;

        public void Consume(RMQMessageBus bus, string queueName)
        {
            _bus = bus;
            _consumer = new EventingBasicConsumer(bus._channel);
            _consumer.Received += HandleMessage;
            _acknowledge = true;
            bus._channel.BasicConsume(queueName, false, _consumer);
        }

        public void Subscribe(RMQMessageBus bus, string exchangeName)
        {
            _bus = bus;

            var queueName = _bus._channel.QueueDeclare().QueueName;
            _bus._channel.QueueBind(queueName, exchangeName, string.Empty);

            _consumer = new EventingBasicConsumer(bus._channel);
            _consumer.Received += HandleMessage;
            _acknowledge = false;
            _bus._channel.BasicConsume(queueName, true, _consumer);
        }

        public abstract void HandleMessage(object sender, BasicDeliverEventArgs e);

        public static RMQMessageHandler Create(IMessageBusContainer container, MessageMetadata metadata)
        {
            var genericHandlerType = typeof(RMQMessageHandler<,>);
            var specificHandlerType = genericHandlerType.MakeGenericType(metadata.MessageType, metadata.HandlerType);

            var handler = (RMQMessageHandler)Activator.CreateInstance(specificHandlerType);
            handler._container = container;

            return handler;
        }
    }
}
