using NServiceBus.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus.ObjectBuilder.Common;
using NServiceBus.Settings;
using BMF.MessageBus.Core.Interfaces;
using NServiceBus;
using Ninject;
using Microsoft.Practices.Unity;

namespace BMF.MessageBus.NServiceBus
{
    public class NSBContainer : ContainerDefinition
    {
        ContainerDefinition _builder;

        public NSBContainer()
        { }

        public override IContainer CreateContainer(ReadOnlySettings settings)
        {
            IMessageBusContainer container;
            if (settings.TryGet<IMessageBusContainer>("MessageBusContainer", out container))
            {
                if (container.NativeContainer is IKernel)
                {
                    _builder = new NinjectBuilder();
                }
                else if (container.NativeContainer is IUnityContainer)
                {
                    _builder = new UnityBuilder();
                }

                if (_builder != null)
                {
                    return _builder.CreateContainer(settings);
                }
            }

            return null;
        }
    }
}
