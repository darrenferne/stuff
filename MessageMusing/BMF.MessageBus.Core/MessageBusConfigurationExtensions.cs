using BMF.MessageBus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core
{
    public static class MessageBusConfigurationExtensions
    {
        public static IMessageBusConfiguration HostName(this IMessageBusConfiguration config, string host)
        {
            config.HostName = host;
            return config;
        }

        public static IMessageBusConfiguration EndpointName(this IMessageBusConfiguration config, string endpoint)
        {
            config.EndpointName = endpoint;
            return config;
        }

        public static IMessageBusConfiguration ErrorQueueName(this IMessageBusConfiguration config, string errorQueue)
        {
            config.ErrorQueueName = errorQueue;
            return config;
        }

        public static IMessageBusConfiguration MessageDefinition(this IMessageBusConfiguration config, MessageMetadata definition)
        {
            if (config.MessageDefinitions == null)
                config.MessageDefinitions = new List<MessageMetadata>();

            config.MessageDefinitions.Add(definition);
            return config;
        }

        public static IMessageBusConfiguration AddMessageDefinition<T_message>(this IMessageBusConfiguration config, Action<MessageMetadata<T_message>> configureDefinition)
        {
            if (config.MessageDefinitions == null)
                config.MessageDefinitions = new List<MessageMetadata>();

            var definition = new MessageMetadata<T_message>();
            configureDefinition(definition);
            config.MessageDefinitions.Add(definition);
            return config;
        }
    }
}
