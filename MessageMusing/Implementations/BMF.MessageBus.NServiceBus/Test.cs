using BMF.MessageBus.Core;
using BMF.MessageBus.Core.Interfaces;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.NServiceBus
{
    public class Test : ISpecifyMessageHandlerOrdering
    {
        Type[] _handlers;

        public Test()
        {
            var nsbHandlerType = typeof(NSBMessageHandler<,>);
            
            _handlers = NSBGlobalConfiguration.Config.MessageDefinitions
                .Where(md => md.HandlerType != null)
                .Select(md => nsbHandlerType.MakeGenericType(md.MessageType, md.HandlerType))
                .ToArray();
        }

        public void SpecifyOrder(Order order)
        {
            order.Specify(_handlers);
        }
    }
}
