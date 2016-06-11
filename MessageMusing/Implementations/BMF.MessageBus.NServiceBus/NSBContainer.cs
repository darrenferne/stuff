using NServiceBus.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus.ObjectBuilder.Common;
using NServiceBus.Settings;

namespace BMF.MessageBus.NServiceBus
{
    public class NSBContainer : ContainerDefinition
    {
        public override IContainer CreateContainer(ReadOnlySettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
