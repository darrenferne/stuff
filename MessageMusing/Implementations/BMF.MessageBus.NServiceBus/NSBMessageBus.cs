using BMF.MessageBus.Core.Interfaces;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.NServiceBus
{
    public class NSBMessageBus : IMessageBus, IDisposable
    {
        internal IMessageBusSerialiser _serialiser;
        internal BusConfiguration _nsbConfiguration;
        internal IStartableBus _factory;
        internal IBus _bus; 

        private bool _disposed;

        public NSBMessageBus(IMessageBusConfiguration configuration)
        {
            _serialiser = configuration.Serialiser;

            _nsbConfiguration = new BusConfiguration();
            _nsbConfiguration.EndpointName(configuration.Host);
            _nsbConfiguration.UseSerialization(typeof(NSBSerialiser));
            _nsbConfiguration.EnableInstallers();
            _nsbConfiguration.UsePersistence<InMemoryPersistence>();
            _nsbConfiguration.LoadMessageHandlers(configuration.MessageDefinitions);

            ConventionsBuilder builder = _nsbConfiguration.Conventions();
            builder.DefiningCommandsAs(t => configuration.MessageDefinitions.SingleOrDefault(md => md.MessageType == t && md.MessageAction == Core.MessageAction.Command) != null);
            builder.DefiningEventsAs(t => configuration.MessageDefinitions.SingleOrDefault(md => md.MessageType == t && md.MessageAction == Core.MessageAction.Event) != null);
            builder.DefiningMessagesAs(t => configuration.MessageDefinitions.SingleOrDefault(md => md.MessageType == t && md.MessageAction == Core.MessageAction.Message) != null);
            builder.DefiningExpressMessagesAs(t => configuration.MessageDefinitions.SingleOrDefault(md => md.MessageType == t && md.ExpressMe == true) != null);

            _factory = Bus.Create(_nsbConfiguration);
            _factory.Subscribe(configuration.MessageDefinitions);
        }

        
        private void Dispose(bool destruct)
        {
            if (!_disposed)
            {
                if (_bus != null)
                {
                    _bus.Dispose();
                    _bus = null;
                }

                if (_factory != null)
                {
                    _factory.Dispose();
                    _factory = null;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Start()
        {
            _bus = _factory.Start();
        }

        public void Stop()
        {
            if (_bus != null)
            {
                _bus.Dispose();
                _bus = null;
            }
        }

        public void Publish<T_message>(T_message message)
        {
            _bus.Publish<T_message>(message);
        }

        public void Subscribe(Type messageType)
        {
            _bus.Subscribe(messageType);
        }

        public void Subscribe<T_message>()
        {
            _bus.Subscribe<T_message>();
        }

        public void Unsubscribe(Type messageType)
        {
            _bus.Unsubscribe(messageType);
        }

        public void Unsubscribe<T_message>()
        {
            _bus.Unsubscribe<T_message>();
        }
    }
}
