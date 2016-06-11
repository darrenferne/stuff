using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core
{
    public class MessageMetadata
    {
        private Type _messageType;

        public MessageMetadata(Type messageType)
        {
            _messageType = messageType;
        }

        public Type MessageType { get { return _messageType; } }
        public MessageAction MessageAction { get; set; }
        public bool ExpressMe { get; set; }
        public TimeSpan Lifetime { get; set; }
    }

    public class MessageMetadata<T_message> : MessageMetadata
    {
        public MessageMetadata()
            : base(typeof(T_message))
        { }
    }
}
