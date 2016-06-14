using BMF.MessageBus.Core;
using BMF.MessageBus.Core.Interfaces;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.WindowsServiceBus
{
    public class WSBMessageHandler<T_message, T_handler> : WSBMessageHandler where T_handler : MessageHandler<T_message>, new()
    {
        public WSBMessageHandler()
        { }

        public override void HandleMessage(BrokeredMessage brokeredMessage)
        {
            byte[] body = brokeredMessage.GetBody<byte[]>();
            T_message message = _bus._serialiser.Deserialise<T_message>(body);

            var handler = _container.ResolveType<T_handler>();
            handler.Bus = _bus;

            try
            {
                handler.HandleMessage(message);

                //if (_subscriptionClient != null)
                //    brokeredMessage.Complete();
            }
            catch
            {
                //if (_subscriptionClient != null)
                //    brokeredMessage.Abandon();
            }
        }
    }

    public abstract class WSBMessageHandler
    {
        protected IMessageBusContainer _container;
        protected WSBMessageBus _bus;
        protected QueueClient _queueClient;
        protected SubscriptionClient _subscriptionClient;

        public void Consume(WSBMessageBus bus, string queueName)
        {
            _bus = bus;

            _queueClient = _bus._messageFactory.CreateQueueClient(queueName);
            _queueClient.OnMessage(HandleMessage);
        }

        public void Subscribe(WSBMessageBus bus, string endpointName, string topicName)
        {
            _bus = bus;

            if (!_bus._namespaceManager.SubscriptionExists(topicName, endpointName))
                _bus._namespaceManager.CreateSubscription(topicName, endpointName);

            _subscriptionClient = _bus._messageFactory.CreateSubscriptionClient(topicName, endpointName);
            _subscriptionClient.OnMessage(HandleMessage);
        }

        public abstract void HandleMessage(BrokeredMessage message);

        public static WSBMessageHandler Create(IMessageBusContainer container, MessageMetadata metadata)
        {
            var genericHandlerType = typeof(WSBMessageHandler<,>);
            var specificHandlerType = genericHandlerType.MakeGenericType(metadata.MessageType, metadata.HandlerType);

            var handler = (WSBMessageHandler)Activator.CreateInstance(specificHandlerType);
            handler._container = container;

            return handler;
        }
    }
}
