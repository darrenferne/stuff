using BMF.MessageBus.Core;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var bmfHandlerType = typeof(MessageHandler<>);
                        
            _handlers = messageDefinitions.Select(md =>
            {
                var bmfMessageHandlerType = bmfHandlerType.MakeGenericType(md.MessageType);
                var nsbMessageHandlerType = nsbHandlerType.MakeGenericType(md.MessageType, bmfMessageHandlerType);
                return nsbMessageHandlerType;
            }).ToArray();
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

            var loadHanderExtensionsType = typeof(LoadMessageHandlersExtentions);
            var busConfigurationType = typeof(BusConfiguration);
            var loadHandlersMethod = loadHanderExtensionsType.GetMethods().FirstOrDefault(m => {
                if (m.Name == "LoadMessageHandlers" && m.IsGenericMethod)
                {
                    var mthParams = m.GetParameters();
                    return mthParams.Length == 2 && mthParams[0].ParameterType == busConfigurationType && mthParams[1].ParameterType.IsGenericType;
                }
                else return false;
            });

            var loadOurHandlersMethod = loadHandlersMethod.MakeGenericMethod(_handlers[0]);

            loadOurHandlersMethod.Invoke(null, new object[] { configuration, firstForOurHandlers });
        }
    }
}
