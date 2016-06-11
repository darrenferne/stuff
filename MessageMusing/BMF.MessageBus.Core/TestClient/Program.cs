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
            IMessageBusSerialiser serialiser = new JsonSerialiser();
            IMessageBusConfiguration configuration = new MessageBusConfiguration("client", serialiser,  new MessageMetadata<TestMessage>());
            
            var bus = new NSBMessageBus(configuration);

            bus.Start();

            SendMessages(bus);
        }

        static void SendMessages(NSBMessageBus bus)
        {
            Console.WriteLine("Press enter to send a message");
            Console.WriteLine("Press any key to exit");

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key != ConsoleKey.Enter)
                    return;
                
                var id = Guid.NewGuid();

                var message = new TestMessage
                {
                    Id = Guid.NewGuid(),
                    Message = "Hello World"
                };

                bus.Publish(message);
                Console.WriteLine($"Published a new Test message with id: {id.ToString("N")}");
            }
        }
    }
}
