using BMF.MessageBus.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.NServiceBus
{
    public class NSBBusStartedHandler : NSBMessageHandler<BusStarted, BusStartedHandler>
    {
        public NSBBusStartedHandler(NSBMessageBus bus, string queueName)
            : base(bus, queueName)
        { }
    }
}
