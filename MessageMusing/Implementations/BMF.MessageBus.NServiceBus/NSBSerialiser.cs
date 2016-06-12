using BMF.MessageBus.Core.Interfaces;
using NServiceBus.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BMF.MessageBus.NServiceBus
{
    internal class NSBSerialiser : IMessageSerializer
    {
        IMessageBusSerialiser _serialiser;

        //public NSBSerialiser()
        //{ }

        public NSBSerialiser(IMessageBusSerialiser serialiser)
        {
            _serialiser = serialiser;
        }

        public string ContentType
        {
            get { return string.Empty; }
        }

        public object[] Deserialize(Stream stream, IList<Type> messageTypes = null)
        {
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);
            var message = _serialiser.Deserialise(messageTypes[0], buffer);

            return new object[] { message };
        }

        public void Serialize(object message, Stream stream)
        {
            var data = _serialiser.Serialise(message);
            stream.Write(data, 0, data.Length);
        }
    }
}
