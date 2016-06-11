using BMF.MessageBus.Core;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.NServiceBus
{
    internal static class NSBExtensions
    {
        public static void LoadMessageHandlers(this BusConfiguration configuration, NSBMessageHandlerTypes handlers)
        {
            handlers.LoadMessageHandlers(configuration);
        }

        public static void LoadMessageHandlers(this BusConfiguration configuration, IList<MessageMetadata> messageDefinitions)
        {
            var handlers = new NSBMessageHandlerTypes(messageDefinitions);
            configuration.LoadMessageHandlers(handlers);
        }

        public static void Subscribe(this IStartableBus factory, IList<MessageMetadata> messageDefinitions)
        {
            foreach (var md in messageDefinitions.Where(md => md.MessageAction == MessageAction.Event))
            {
                factory.Subscribe(md.MessageType);
            }
        }
    }
}
