using BMF.MessageBus.ActiveMq;
using BMF.MessageBus.Core;
using BMF.MessageBus.Core.Interfaces;
using BMF.MessageBus.NServiceBus;
using BMF.MessageBus.RabbitMq;
using BMF.MessageBus.Serialisers;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDomain;

namespace TestService
{
    class Program
    {
        static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel();
            IMessageBusConfiguration configuration = new MessageBusConfiguration("", "TestService", "ServiceErrors", 
                new MessageMetadata<TestMessage>() { MessageAction = MessageAction.Event, QueueName = "TestService" }, 
                new MessageMetadata<TestRequest>() { MessageAction = MessageAction.Command, HandlerType = typeof(TestRequestHandler) });

            kernel.Bind<IMessageBusSerialiser>().To<JsonSerialiser>();
            kernel.Bind<IMessageBusConfiguration>().ToConstant(configuration).InSingletonScope();

            IMessageBusContainer container = new NinjectContainer(kernel);
            
            //var bus = new NSBMessageBus(container, configuration);
            //var bus = new RMQMessageBus(container, configuration);
            var bus = new AMQMessageBus(container, configuration);

            bus.Start();

            Console.WriteLine("Press Enter to quit");
            Console.ReadLine();

            //SendMessages(bus);
        }
        
        static void SendMessages(IMessageBus bus)
        {
            Console.WriteLine("Press Enter to send a message");
            Console.WriteLine("Press any key to quit");

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key != ConsoleKey.Enter)
                    return;

                Console.WriteLine("Publishing 1000 new Test messages");
                var sw = Stopwatch.StartNew();
                for (int x = 0; x < 1000; x++)
                {
                    var id = Guid.NewGuid();
                    var message = new TestMessage
                    {
                        Id = Guid.NewGuid(),
                        Message = "Hello World".PadRight(10000) + "Ha!"
                    };

                    bus.Publish(message);
                    //Console.WriteLine($"Published a new Test message with id: {id.ToString("N")}");
                }
                sw.Stop();
                Console.WriteLine($"Published 1000 new Test messages in {sw.ElapsedMilliseconds / 1000}s");
            }
        }
    }
}
