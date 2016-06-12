using BMF.MessageBus.Core;
using BMF.MessageBus.Core.Interfaces;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.NServiceBus
{
    public class NSBRouteConfiguration : IConfigurationSource
    {
        IList<MessageMetadata> _messageDefinitions;

        public NSBRouteConfiguration(IMessageBusConfiguration configuration)
        {
            _messageDefinitions = configuration.MessageDefinitions;
        }

        public T GetConfiguration<T>() where T : class, new()
        {
            if (typeof(T) == typeof(UnicastBusConfig))
            {
                var config = (UnicastBusConfig)ConfigurationManager.GetSection(typeof(UnicastBusConfig).Name);
                if (config == null)
                {
                    config = new UnicastBusConfig
                    {
                        MessageEndpointMappings = new MessageEndpointMappingCollection()
                    };
                }

                foreach (var definition in _messageDefinitions.Where(md => md.MessageAction == MessageAction.Event))
                {
                    config.MessageEndpointMappings.Add(
                        new MessageEndpointMapping
                        {
                            AssemblyName = definition.MessageType.Assembly.FullName,
                            TypeFullName = definition.MessageType.FullName,
                            Endpoint = definition.QueueName
                        });
                    return config as T;
                }
            }
            
            return ConfigurationManager.GetSection(typeof(T).Name) as T;
        }
    }
}
