using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core.Interfaces
{
    public interface IMessageBusConfiguration
    {
        string HostName { get; }
        string EndpointName { get; }
        string ErrorQueueName { get; }
        IList<MessageMetadata> MessageDefinitions { get; }
    }
}
