using BMF.MessageBus.Core;
using BMF.MessageBus.Core.Interfaces;
using Microsoft.Practices.Unity;
using Ninject;
using NServiceBus;
using NServiceBus.Container;
using NServiceBus.ObjectBuilder;
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

        public static void LoadMessageHandlers(this BusConfiguration nsbConfiguration, IMessageBusConfiguration bmfConfiguration)
        {
            var handlers = new NSBMessageHandlerTypes(bmfConfiguration);
            nsbConfiguration.LoadMessageHandlers(handlers);
        }

        public static void Subscribe(this IStartableBus factory, IList<MessageMetadata> messageDefinitions)
        {
            foreach (var md in messageDefinitions.Where(md => md.MessageAction == MessageAction.Event && md.HandlerType != null))
            {
                factory.Subscribe(md.MessageType);
            }
        }

        public static void Configure(this ContainerCustomizations customizations, IMessageBusContainer container, IMessageBusConfiguration configuration)
        {
            customizations.Settings.Set("MessageBusContainer", container);
            customizations.Settings.Set("MessageBusConfiguration", configuration);

            var nativeContainer = container.NativeContainer;
            if (nativeContainer is IKernel)
                customizations.Settings.Set("ExistingKernel", nativeContainer);
            else if (nativeContainer is IUnityContainer)
                customizations.Settings.Set("ExistingContainer", nativeContainer);
        }

        public static void ConfigureComponents(this IConfigureComponents componentConfig, IMessageBus bus, IMessageBusConfiguration configuration)
        {
            var nsbHandlerType = typeof(NSBMessageHandler<,>);

            var handlers = configuration.MessageDefinitions
                .Where(md => md.HandlerType != null)
                .Select(md => nsbHandlerType.MakeGenericType(md.MessageType, md.HandlerType));

            foreach (var handler in handlers)
            {
                componentConfig.ConfigureComponent(handler, DependencyLifecycle.InstancePerCall).ConfigureProperty("Bus", bus);
            }
        }
    }
}
