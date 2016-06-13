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
        private string _host;
        private string _endpoint;
        private string _errorQueue;
        private List<MessageMetadata> _messageDefinitions;
        
        public MessageBusConfiguration(string host, string endpoint, string errorQueue, params MessageMetadata[] definitions)
            : this(host, endpoint, errorQueue, definitions as IEnumerable<MessageMetadata>)
        { }

        public MessageBusConfiguration(string host, string endpoint, string errorQueue, IEnumerable<MessageMetadata> definitions)
        {
            _host = host;
            _endpoint = endpoint;
            _errorQueue = errorQueue;
            _messageDefinitions = new List<MessageMetadata>(definitions);
        }
        
        public string HostName { get { return _host; } }
        public string EndpointName { get { return _endpoint; } }
        public string ErrorQueueName { get { return _errorQueue; } }
        public IList<MessageMetadata> MessageDefinitions { get { return _messageDefinitions; } }
    }
}
