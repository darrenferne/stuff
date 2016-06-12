using BMF.MessageBus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.NServiceBus
{
    public static class NSBGlobalConfiguration
    {
        public static IMessageBusConfiguration Config { get; set; } 
    }
}
