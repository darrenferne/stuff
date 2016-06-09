using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core
{
    public class MessageMetadata<T>
    {
        public bool ExpressMe { get; set; }
        public TimeSpan Lifetime { get; set; }
    }
}
