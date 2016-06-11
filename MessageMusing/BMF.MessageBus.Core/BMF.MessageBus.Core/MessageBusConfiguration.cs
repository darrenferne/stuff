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
        private List<MessageMetadata> _messageDefinitions;
        private IMessageBusSerialiser _serialiser;

        public MessageBusConfiguration(string host, IMessageBusSerialiser serialiser, params MessageMetadata[] definitions)
            : this(host, serialiser, definitions as IEnumerable<MessageMetadata>)
        { }

        public MessageBusConfiguration(string host, IMessageBusSerialiser serialiser, IEnumerable<MessageMetadata> definitions)
        {
            _host = host;
            _serialiser = serialiser;
            _messageDefinitions = new List<MessageMetadata>(definitions);
        }

        public string Host { get { return _host; } }
        public IList<MessageMetadata> MessageDefinitions { get { return _messageDefinitions; } }
        public IMessageBusSerialiser Serialiser
        {
            get { return _serialiser; }
            set { _serialiser = value; }
        }
    }
}
