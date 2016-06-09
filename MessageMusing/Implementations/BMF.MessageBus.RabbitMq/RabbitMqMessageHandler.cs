using BMF.MessageBus.Core;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.RabbitMq
{
    public class RabbitMqMessageHandler<T_message> : MessageHandler<T_message>
    {
        RabbitMqBus _bus;
        EventingBasicConsumer _consumer;

        public RabbitMqMessageHandler(RabbitMqBus bus, string queueName)
        {
            _bus = bus;
            _consumer = new EventingBasicConsumer(_bus._model);
            _consumer.Received += internalHandler;

            _bus._model.BasicConsume(queueName, false, _consumer);
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
