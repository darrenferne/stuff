using BMF.MessageBus.ActiveMq;
using BMF.MessageBus.Core;
using BMF.MessageBus.Core.Interfaces;
using BMF.MessageBus.NServiceBus;
using BMF.MessageBus.RabbitMq;
using BMF.MessageBus.Serialisers;
using BMF.MessageBus.WindowsServiceBus;
using Ninject;
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
            IKernel kernel = new StandardKernel();
            IMessageBusConfiguration configuration = 
                MessageBusConfiguration.Create()
                    .EndpointName("TestClient")
                    .ErrorQueueName("ClientErrors")
                    .AddMessageDefinition<TestRequest>(d => d.MessageAction = MessageAction.Command);
                
            kernel.Bind<IMessageBusSerialiser>().To<JsonSerialiser>();
            kernel.Bind<IMessageBusConfiguration>().ToConstant(configuration).InSingletonScope();

            IMessageBusContainer container = new NinjectContainer(kernel);

            //var bus = new NSBMessageBus(container, configuration);
            //var bus = new RMQMessageBus(container, configuration);
            //var bus = new AMQMessageBus(container, configuration);
            var bus = new WSBMessageBus(container, configuration.HostName("campc-df7.bradyplc.com").ServiceNamespace("TheMagicBus"));

            bus.Start();

            SendMessages(bus);
        }

        static void SendMessages(IMessageBus bus)
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
                var message = new TestRequest
                {
                    Id = Guid.NewGuid(),
                    MessageCount = 1000
                };

                bus.Send("TestService", message);
                Console.WriteLine($"Published a new Test Request message with id: {id.ToString("N")}");
            }
        }
    }
}
