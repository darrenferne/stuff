using BMF.MessageBus.Core.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.RabbitMq
{
    public class RMQMessageBus : IMessageBus, IDisposable
    {
        internal IMessageBusContainer _container;
        internal IMessageBusConfiguration _configuration;
        internal IMessageBusSerialiser _serialiser;
        internal ConnectionFactory _factory;
        internal IConnection _connection;
        internal IModel _channel;
        //internal ConcurrentDictionary<Type, QueueDeclareOk> _messageQueues;
        internal ConcurrentDictionary<Type, RMQMessageHandler> _messageHandlers;
        
        private bool _disposed;
        
        public RMQMessageBus(IMessageBusContainer container, IMessageBusConfiguration configuration)
        {
            _container = container;
            _configuration = configuration;
            _serialiser = container.ResolveType<IMessageBusSerialiser>();

            _factory = new ConnectionFactory()
            {
                HostName = string.IsNullOrEmpty(configuration.HostName) ? "localhost" : configuration.HostName
            };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            foreach (var exchange in _configuration.MessageDefinitions.Where(md => md.MessageAction == Core.MessageAction.Event).Select(md => md.QueueName).Distinct())
            {
                _channel.ExchangeDeclare(exchange, "fanout", true);
            }
        }

        private void CreateQueue(Type messageType, string queueName)
        {
            var ok = _channel.QueueDeclare(queueName, true, false, false, null);
            //_messageQueues.AddOrUpdate(messageType, ok, (k, v) => ok);
        }

        private void InitialiseQueues()
        {
            //_messageQueues = new ConcurrentDictionary<Type, QueueDeclareOk>();
            
            foreach (var md in _configuration.MessageDefinitions)
            {
                var queueName = string.IsNullOrEmpty(md.QueueName) ? _configuration.EndpointName : md.QueueName;
                CreateQueue(md.MessageType, queueName);
            }
        }

        private void InitialiseHandlers()
        {
            _messageHandlers = new ConcurrentDictionary<Type, RMQMessageHandler>();

            foreach (var metadata in _configuration.MessageDefinitions.Where(md => md.HandlerType != null))
            {
                if (!_messageHandlers.ContainsKey(metadata.MessageType))
                {
                    if (metadata != null)
                    {
                        _messageHandlers.TryAdd(metadata.MessageType, RMQMessageHandler.Create(_container, metadata));

                        if (_messageHandlers.ContainsKey(metadata.MessageType) && metadata.MessageAction == Core.MessageAction.Command)
                        {
                            _messageHandlers[metadata.MessageType].Consume(this, _configuration.EndpointName);
                        }
                    }
                }
            }
        }

        public void AutoSubscribe()
        {
            foreach (var metadata in _configuration.MessageDefinitions.Where(md => md.MessageAction == Core.MessageAction.Event && md.HandlerType != null))
            {
                //if (_messageQueues.ContainsKey(metadata.MessageType))
                //{
                //    var messageQueue = _messageQueues[metadata.MessageType];
                    if (_messageHandlers.ContainsKey(metadata.MessageType))
                    {
                        //_messageHandlers[metadata.MessageType].Subscribe(this, messageQueue.QueueName);
                        _messageHandlers[metadata.MessageType].Subscribe(this, metadata.QueueName);
                    }
                //}
            }
        }

        private void Dispose(bool destruct)
        {
            if (!_disposed)
            {
                if (_channel != null)
                {
                    _channel.Dispose();
                    _channel = null;
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

        public void Start()
        {
            InitialiseQueues();
            InitialiseHandlers();
            AutoSubscribe();
        }

        public void Stop()
        {
            _channel.Dispose();
            _channel = null;
        }

        public void Publish<T_message>(T_message message)
        {
            var messageType = typeof(T_message);

            var metadata = _configuration.MessageDefinitions.FirstOrDefault(md => md.MessageType == messageType && md.MessageAction == Core.MessageAction.Event);
            //if (_messageQueues.ContainsKey(messageType))
            //{
                //var messageQueue = _messageQueues[messageType];
                var body = _serialiser.Serialise(message);
                var properties = _channel.CreateBasicProperties();

                properties.Persistent = true;

                _channel.BasicPublish(metadata.QueueName, string.Empty, properties, body);
            //}
        }

        public void Subscribe(Type messageType)
        {
            //if (_messageQueues.ContainsKey(messageType))
            //{
            //    var messageQueue = _messageQueues[messageType];
                
                if (_messageHandlers.ContainsKey(messageType))
                {

                    //_messageHandlers[messageType].Subscribe(this, messageQueue.QueueName);
                    var metadata = _configuration.MessageDefinitions.FirstOrDefault(md => md.MessageType == messageType && md.MessageAction == Core.MessageAction.Event);
                    _messageHandlers[messageType].Subscribe(this, metadata.QueueName);
                }
            //}
        }

        public void Subscribe<T_message>()
        {
            Subscribe(typeof(T_message));
        }

        public void Unsubscribe(Type messageType)
        {
            if (_messageHandlers.ContainsKey(messageType))
            {
                RMQMessageHandler removed;
                _messageHandlers.TryRemove(messageType, out removed);
            }
        }

        public void Unsubscribe<T_message>()
        {
            Unsubscribe(typeof(T_message));
        }

        public void Send<T_message>(string destination, T_message message)
        {
            var messageType = typeof(T_message);

            //if (!_messageQueues.ContainsKey(messageType))
            //{
                CreateQueue(typeof(T_message), destination);
            //}

            //if (_messageQueues.ContainsKey(messageType))
            //{
            //    var messageQueue = _messageQueues[messageType];
                var body = _serialiser.Serialise(message);
                
                //_channel.BasicPublish(string.Empty, messageQueue.QueueName, null, body);
                _channel.BasicPublish(string.Empty, destination, null, body);
            //}
        }

        public void Reply<T_message>(T_message message)
        {
            throw new NotImplementedException();
        }
    }
}
