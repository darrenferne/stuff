using BMF.MessageBus.Core;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.RabbitMq
{
    public class RMQMessageHandler<T_message> : MessageHandler<T_message>
    {
        RMQMessageBus _bus;
        EventingBasicConsumer _consumer;

        public RMQMessageHandler(RMQMessageBus bus, string queueName)
        {
            _bus = bus;
            _consumer = new EventingBasicConsumer(_bus._model);
            _consumer.Received += internalHandler;

            _bus._model.BasicConsume(queueName, false, _consumer);
        }

        public override void HandleMessage(T_message message)
        {
            throw new NotImplementedException();
        }

        private void internalHandler(object sender, BasicDeliverEventArgs e)
        {
            byte[] body = e.Body;
            T_message message = _bus._serialiser.Deserialise<T_message>(body);

            this.HandleMessage(message);

            _consumer.Model.BasicAck(e.DeliveryTag, false);
        }
    }
}
