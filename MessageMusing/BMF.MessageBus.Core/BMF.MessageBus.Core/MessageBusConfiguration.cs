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
        private string _errorQueue;
        private List<MessageMetadata> _messageDefinitions;
        
        public MessageBusConfiguration(string host, string errorQueue, params MessageMetadata[] definitions)
            : this(host, errorQueue, definitions as IEnumerable<MessageMetadata>)
        { }

        public MessageBusConfiguration(string host, string errorQueue, IEnumerable<MessageMetadata> definitions)
        {
            _host = host;
            _errorQueue = errorQueue;
            _messageDefinitions = new List<MessageMetadata>(definitions);
        }
        
        public string Host { get { return _host; } }
        public string ErrorQueue { get { return _errorQueue; } }
        public IList<MessageMetadata> MessageDefinitions { get { return _messageDefinitions; } }
    }
}
