using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core.Interfaces
{
    public interface IMessageBusSerialiser
    {
        byte[] Serialise<T_message>(T_message message);
        byte[] Serialise(object message);

        T_message Deserialise<T_message>(byte[] messageData);
        object Deserialise(Type type, byte[] messageData);
    }
}
