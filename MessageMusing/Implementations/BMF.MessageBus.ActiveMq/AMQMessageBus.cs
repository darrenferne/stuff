using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.Util;
using BMF.MessageBus.Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.ActiveMq
{
    public class AMQMessageBus : IMessageBus, IDisposable
    {
        private readonly string _defaultHost = @"localhost";
        private readonly string _brokerUriTemplate = @"activemq:tcp://{0}:61616/";

        internal IMessageBusContainer _container;
        internal IMessageBusConfiguration _configuration;
        internal IMessageBusSerialiser _serialiser;
        internal IConnectionFactory _factory;
        internal IConnection _connection;
        internal ISession _session;
        internal ConcurrentDictionary<Type, AMQMessageHandler> _messageHandlers;
        internal ConcurrentDictionary<string, IMessageProducer> _messageQueues;

        private bool _disposed;

        public AMQMessageBus(IMessageBusContainer container, IMessageBusConfiguration configuration)
        {
            _container = container;
            _configuration = configuration;
            _serialiser = container.ResolveType<IMessageBusSerialiser>();

            var hostName = string.IsNullOrEmpty(configuration.HostName) ? _defaultHost : configuration.HostName;
            var brokerUri = string.Format(_brokerUriTemplate, hostName);

            _factory = new ConnectionFactory(brokerUri);

            _connection = _factory.CreateConnection();
            _connection.ClientId = _configuration.EndpointName;
            _session = _connection.CreateSession();
        }

        private string GetDestinationName(string queueName, bool forTopic)
        {
            var destinationType = forTopic ? "topic" : "queue";
            return string.Format("{0}://{1}", destinationType, queueName);
        }

        private void CreateQueue(Type messageType, string destinationName)
        {
            var destination = _session.GetDestination(destinationName);
            var producer = _session.CreateProducer(destination);

            producer.DeliveryMode = MsgDeliveryMode.Persistent;
            
            _messageQueues.AddOrUpdate(destinationName, producer, (k,v) => producer);
        }

        private void InitialiseQueues()
        {
            _messageQueues = new ConcurrentDictionary<string, IMessageProducer>();

            foreach (var md in _configuration.MessageDefinitions)
            {
                var queueName = string.IsNullOrEmpty(md.QueueName) ? _configuration.EndpointName : md.QueueName;
                var destinationName = GetDestinationName(queueName, md.MessageAction == Core.MessageAction.Event);

                CreateQueue(md.MessageType, destinationName);
            }
        }

        private void InitialiseHandlers()
        {
            _messageHandlers = new ConcurrentDictionary<Type, AMQMessageHandler>();

            foreach (var metadata in _configuration.MessageDefinitions.Where(md => md.HandlerType != null))
            {
                if (!_messageHandlers.ContainsKey(metadata.MessageType))
                {
                    if (metadata != null)
                    {
                        _messageHandlers.TryAdd(metadata.MessageType, AMQMessageHandler.Create(_container, metadata));

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
                    _messageHandlers[metadata.MessageType].Subscribe(this, _configuration.EndpointName, metadata.QueueName);
                }
                //}
            }
        }

        private void Dispose(bool destruct)
        {
            if (!_disposed)
            {
                if (_session != null)
                {
                    _session.Dispose();
                    _session = null;
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

            _connection.Start();
        }

        public void Stop()
        {
            if (_connection.IsStarted)
                _connection.Stop();
        }

        public void Publish<T_message>(T_message message)
        {
            var messageType = typeof(T_message);
            var metadata = _configuration.MessageDefinitions.FirstOrDefault(md => md.MessageType == messageType && md.MessageAction == Core.MessageAction.Event);
            if (metadata != null)
            {
                var destinationName = GetDestinationName(metadata.QueueName, true);
                if (_messageQueues.ContainsKey(destinationName))
                {
                    var producer = _messageQueues[destinationName];
                    var body = _serialiser.Serialise(message);
                    var messageBytes = _session.CreateBytesMessage(body);

                    producer.Send(messageBytes);
                }
            }
        }

        public void Subscribe(Type messageType)
        {
            if (_messageHandlers.ContainsKey(messageType))
            {
                var metadata = _configuration.MessageDefinitions.FirstOrDefault(md => md.MessageType == messageType && md.MessageAction == Core.MessageAction.Event);
                _messageHandlers[messageType].Subscribe(this, _configuration.EndpointName, metadata.QueueName);
            }
        }

        public void Subscribe<T_message>()
        {
            Subscribe(typeof(T_message));
        }

        public void Unsubscribe(Type messageType)
        {
            if (_messageHandlers.ContainsKey(messageType))
            {
                AMQMessageHandler removed;
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
            var metadata = _configuration.MessageDefinitions.FirstOrDefault(md => md.MessageType == messageType && md.MessageAction == Core.MessageAction.Command);
            if (metadata != null)
            {
                var destinationName = GetDestinationName(destination, false);
                if (!_messageQueues.ContainsKey(destinationName))
                {
                    CreateQueue(messageType, destinationName);
                }

                if (_messageQueues.ContainsKey(destinationName))
                {
                    var producer = _messageQueues[destinationName];
                    var body = _serialiser.Serialise(message);
                    var messageBytes = _session.CreateBytesMessage(body);
                    
                    producer.Send(messageBytes);
                }
            }
        }
    }
}
