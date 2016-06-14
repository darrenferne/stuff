using BMF.MessageBus.Core.Interfaces;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.WindowsServiceBus
{
    public class WSBMessageBus : IMessageBus, IDisposable
    {
        private readonly int _defaultRuntimePort = 9354;
        private readonly int _defaultManagementPort = 9355;
        private readonly string _defaultHost = "localhost";

        internal IMessageBusContainer _container;
        internal IMessageBusConfiguration _configuration;
        internal IMessageBusSerialiser _serialiser;

        internal MessagingFactory _messageFactory;
        internal NamespaceManager _namespaceManager;
        internal ConcurrentDictionary<Type, WSBMessageHandler> _messageHandlers;

        private bool _disposed;

        public WSBMessageBus(IMessageBusContainer container, IMessageBusConfiguration configuration)
        {
            _container = container;
            _configuration = configuration;
            _serialiser = container.ResolveType<IMessageBusSerialiser>();

            var extendedConfig = configuration.ExtendedConfiguration as WSBExtendedConfiguration;
            if(extendedConfig == null)
                extendedConfig = new WSBExtendedConfiguration();

            var connectStringBuilder = new ServiceBusConnectionStringBuilder();
            var hostName = string.IsNullOrEmpty(_configuration.HostName) ? _defaultHost : _configuration.HostName;
            var managementPort = extendedConfig.HttpPort == 0 ? _defaultManagementPort : extendedConfig.HttpPort;
            var runtimePort = extendedConfig.TcpPort == 0 ? _defaultRuntimePort : extendedConfig.TcpPort;

            connectStringBuilder.ManagementPort = managementPort;
            connectStringBuilder.RuntimePort = runtimePort;
            connectStringBuilder.Endpoints.Add((new UriBuilder() { Scheme = "sb", Host = hostName, Path = extendedConfig.ServiceNamespace }).Uri);
            connectStringBuilder.StsEndpoints.Add((new UriBuilder() { Scheme = "https", Host = hostName, Port = managementPort, Path = extendedConfig.ServiceNamespace }).Uri);

            var connectionString = connectStringBuilder.ToString();

            _messageFactory = MessagingFactory.CreateFromConnectionString(connectionString);
            _namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            
            foreach (var queueName in _configuration.MessageDefinitions.Where(md => md.MessageAction == Core.MessageAction.Event).Select(md => md.QueueName).Distinct())
            {
                CreateTopic(queueName);
            }
        }

        private string GetDestinationName(string queueName, bool forTopic)
        {
            var destinationType = forTopic ? "topic" : "queue";
            return string.Format("{0}-{1}", destinationType, queueName);
        }

        private void CreateTopic(string topicName)
        {
            var destination = GetDestinationName(topicName, true);
            if (!_namespaceManager.TopicExists(destination))
                _namespaceManager.CreateTopic(destination);
        }

        private void CreateQueue(string queueName)
        {
            var destination = GetDestinationName(queueName, false);
            if (!_namespaceManager.QueueExists(destination))
                _namespaceManager.CreateQueue(destination);
        }

        private void InitialiseQueues()
        {
            foreach (var md in _configuration.MessageDefinitions.Where(md => md.MessageAction == Core.MessageAction.Command && md.QueueName != null))
            {
                CreateQueue(md.QueueName);
            }
        }

        private void InitialiseHandlers()
        {
            _messageHandlers = new ConcurrentDictionary<Type, WSBMessageHandler>();

            foreach (var metadata in _configuration.MessageDefinitions.Where(md => md.HandlerType != null))
            {
                if (!_messageHandlers.ContainsKey(metadata.MessageType))
                {
                    if (metadata != null)
                    {
                        _messageHandlers.TryAdd(metadata.MessageType, WSBMessageHandler.Create(_container, metadata));

                        if (_messageHandlers.ContainsKey(metadata.MessageType) && metadata.MessageAction == Core.MessageAction.Command)
                        {
                            var destination = GetDestinationName(_configuration.EndpointName, false);
                            _messageHandlers[metadata.MessageType].Consume(this, destination);
                        }
                    }
                }
            }
        }

        public void AutoSubscribe()
        {
            foreach (var metadata in _configuration.MessageDefinitions.Where(md => md.MessageAction == Core.MessageAction.Event && md.HandlerType != null))
            {
                if (_messageHandlers.ContainsKey(metadata.MessageType))
                {
                    var destination = GetDestinationName(metadata.QueueName, true);
                    _messageHandlers[metadata.MessageType].Subscribe(this, _configuration.EndpointName, destination);
                }
            }
        }

        private void Dispose(bool destruct)
        {
            if (!_disposed)
            {
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
            if (_messageFactory != null)
                _messageFactory.Close();
        }

        public void Publish<T_message>(T_message message)
        {
            var messageType = typeof(T_message);
            var metadata = _configuration.MessageDefinitions.FirstOrDefault(md => md.MessageType == messageType && md.MessageAction == Core.MessageAction.Event);
            if (metadata != null)
            {
                //CreateTopic(metadata.QueueName);
                var body = _serialiser.Serialise(message);
                BrokeredMessage brokeredMessage = new BrokeredMessage(body);

                var destination = GetDestinationName(metadata.QueueName, true);
                TopicClient topicClient = _messageFactory.CreateTopicClient(destination);
                topicClient.Send(brokeredMessage);
            }
        }

        public void Subscribe(Type messageType)
        {
            if (_messageHandlers.ContainsKey(messageType))
            {
                var metadata = _configuration.MessageDefinitions.FirstOrDefault(md => md.MessageType == messageType && md.MessageAction == Core.MessageAction.Event);
                var destination = GetDestinationName(metadata.QueueName, true);
                _messageHandlers[messageType].Subscribe(this, _configuration.EndpointName, destination);
            }
        }

        public void Subscribe<T_message>()
        {
            Subscribe(typeof(T_message));
        }

        public void Unsubscribe(Type messageType)
        {
            var metadata = _configuration.MessageDefinitions.FirstOrDefault(md => md.MessageType == messageType && md.MessageAction == Core.MessageAction.Event);
            if (metadata != null)
            {
                if (_namespaceManager.SubscriptionExists(metadata.QueueName, _configuration.EndpointName))
                    _namespaceManager.DeleteSubscription(metadata.QueueName, _configuration.EndpointName);
            }
        }

        public void Unsubscribe<T_message>()
        {
            Unsubscribe(typeof(T_message));
        }

        public void Send<T_message>(string destination, T_message message)
        {
            CreateQueue(destination);
            destination = GetDestinationName(destination, false);

            var body =_serialiser.Serialise(message);
            BrokeredMessage brokeredMessage = new BrokeredMessage(body);
            
            QueueClient queueClient = _messageFactory.CreateQueueClient(destination);
            queueClient.Send(brokeredMessage);
        }
    }
}
