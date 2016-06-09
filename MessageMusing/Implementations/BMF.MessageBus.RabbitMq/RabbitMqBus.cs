using BMF.MessageBus.Core.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.RabbitMq
{
    public class RabbitMqBus : IMessageBus, IDisposable
    {
        internal ISerialiser _serialiser;
        internal ConnectionFactory _factory;
        internal IConnection _connection;
        internal IModel _model;
        internal Dictionary<string, QueueDeclareOk> _queues;

        private bool _disposed;
        
        public RabbitMqBus(ISerialiser serialiser)
        {
            _serialiser = serialiser;
            _factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();
            _queues = new Dictionary<string, QueueDeclareOk>();
        }

        private void CreateQueue(string name)
        {
            var ok = _model.QueueDeclare(queue: name, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _queues.Add(name, ok);
        }
                
        private void Dispose(bool destruct)
        {
            if (!_disposed)
            {
                if (_model != null)
                {
                    _model.Dispose();
                    _model = null;
                }

                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }

                if (_factory != null)
                    _factory = null;
                
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Publish<T_message>(T_message message)
        {
            var body = _serialiser.Serialise(message);

            var properties = _model.CreateBasicProperties();
            properties.Persistent = true;

            _model.BasicPublish(exchange: "",
                                 routingKey: _queues.FirstOrDefault().Key,
                                 basicProperties: properties,
                                 body: body);
        }

        public void Subscribe<T_message>()
        {
            throw new NotImplementedException();
        }

        public int Subscribe<T_message>(Action<T_message> action)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T_message>()
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T_message>(int subscription)
        {
            throw new NotImplementedException();
        }
    }
}
