using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core.Interfaces
{
    public interface IMessageBusConfiguration
    {
        string HostName { get; set; }
        string EndpointName { get; set; }
        string ErrorQueueName { get; set; }
        IList<MessageMetadata> MessageDefinitions { get; set; }
        IMessageBusConfigurationExtension ExtendedConfiguration { get; set; }
    }
}
