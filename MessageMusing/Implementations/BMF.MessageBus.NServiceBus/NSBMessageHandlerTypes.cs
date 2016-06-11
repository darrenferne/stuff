using BMF.MessageBus.Core;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.NServiceBus
{
    internal class NSBMessageHandlerTypes 
    {
        Type[] _handlers;

        public NSBMessageHandlerTypes(IList<MessageMetadata> messageDefinitions)
        {
            var nsbHandlerType = typeof(NSBMessageHandler<,>);
            var bmfHandlerType = typeof(IMessageHandler<>);
                        
            _handlers = messageDefinitions.Select(md => nsbHandlerType.MakeGenericType(md.MessageType, bmfHandlerType.MakeGenericType(md.MessageType))).ToArray();
        }

        public IEnumerable<Type> Types { get { return _handlers; } }

        public void LoadMessageHandlers(BusConfiguration configuration, bool autoSubscribe = true)
        {
            var nsbFirstType = typeof(First<>);
            var nsbFirstForOurHandlersType = nsbFirstType.MakeGenericType(_handlers[0]);
            var andThenMethod = nsbFirstForOurHandlersType.GetMethod("AndThen");

            var firstForOurHandlers = Activator.CreateInstance(nsbFirstForOurHandlersType);
            for (int handler = 1; handler < _handlers.Length; handler++)
            {
                andThenMethod.MakeGenericMethod(_handlers[handler]);
                andThenMethod.Invoke(firstForOurHandlers, null);
            }

            var nsbBusConfigurationType = configuration.GetType();
            var loadHandlersMethod = nsbBusConfigurationType.GetMethod("LoadMessageHandlers");
            var loadOurHandlersMethod = loadHandlersMethod.MakeGenericMethod(nsbFirstForOurHandlersType);

            loadOurHandlersMethod.Invoke(configuration, new object[] { firstForOurHandlers });
        }
    }
}
