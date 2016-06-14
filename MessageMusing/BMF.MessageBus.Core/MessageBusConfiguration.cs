using BMF.MessageBus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core
{
    public class MessageBusConfiguration : IMessageBusConfiguration
    {
        public static MessageBusConfiguration Create()
        {
            return new MessageBusConfiguration();
        }

        public MessageBusConfiguration()
        { }
        
        public string HostName { get; set; } 
        public string EndpointName { get; set; }
        public string ErrorQueueName { get; set; }
        public IList<MessageMetadata> MessageDefinitions { get; set; }
        public IMessageBusConfigurationExtension ExtendedConfiguration { get; set; }
    }
}
