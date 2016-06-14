using BMF.MessageBus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.WindowsServiceBus
{
    public static class WSBConfigurationExtensions
    {
        public static IMessageBusConfiguration HttpPort(this IMessageBusConfiguration config, int port)
        {
            if (config.ExtendedConfiguration == null)
                config.ExtendedConfiguration = new WSBExtendedConfiguration();

            var wsbConf = config.ExtendedConfiguration as WSBExtendedConfiguration;
            if (wsbConf != null)
            {
                wsbConf.HttpPort = port;
            }

            return config;
        }

        public static IMessageBusConfiguration TcpPort(this IMessageBusConfiguration config, int port)
        {
            if (config.ExtendedConfiguration == null)
                config.ExtendedConfiguration = new WSBExtendedConfiguration();

            var wsbConf = config.ExtendedConfiguration as WSBExtendedConfiguration;
            if (wsbConf != null)
            {
                wsbConf.TcpPort = port;
            }

            return config;
        }

        public static IMessageBusConfiguration ServiceNamespace(this IMessageBusConfiguration config, string serviceNamespace)
        {
            if (config.ExtendedConfiguration == null)
                config.ExtendedConfiguration = new WSBExtendedConfiguration();

            var wsbConf = config.ExtendedConfiguration as WSBExtendedConfiguration;
            if (wsbConf != null)
            {
                wsbConf.ServiceNamespace = serviceNamespace;
            }

            return config;
        }
    }
}
