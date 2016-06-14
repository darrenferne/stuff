using BMF.MessageBus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.WindowsServiceBus
{
    public static class WSBMessageExtensions
    {
        public static Stream ToStream<T_message>(this T_message message, IMessageBusSerialiser serialiser)
        {
            var bytes = serialiser.Serialise(message);
            return new MemoryStream(bytes, false);
        }
    }
}
