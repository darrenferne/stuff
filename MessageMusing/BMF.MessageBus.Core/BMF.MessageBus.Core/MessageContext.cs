using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core
{
    public class MessageContext
    {
        public string SystemType { get; set; }
        public string SystemIdentifier { get; set; }
        public Guid MessageId { get; set; }
        public DateTime CreatedTimestampUTC { get; set; }
        public DateTime? SentTimestampUTC { get; set; }
    }
}
