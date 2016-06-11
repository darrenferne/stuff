using BMF.MessageBus.Core;
using BMF.MessageBus.Core.Interfaces;
using BMF.MessageBus.NServiceBus;
using BMF.MessageBus.Serialisers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDomain;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IMessageBusConfiguration configuration = new MessageBusConfiguration("client", new MessageMetadata<TestMessage>());
            IMessageBusSerialiser serialiser = new JsonSerialiser();

            var bus = new NSBMessageBus(configuration, serialiser);

            
        }
    }
}
