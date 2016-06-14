using BMF.MessageBus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.WindowsServiceBus
{
    public class WSBExtendedConfiguration : IMessageBusConfigurationExtension
    {
        public int HttpPort { get; set; }
        public int TcpPort { get; set; }
        public string ServiceNamespace { get; set; }
    }
}
