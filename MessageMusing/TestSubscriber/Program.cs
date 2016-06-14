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

namespace TestSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel();
            IMessageBusConfiguration configuration = 
                MessageBusConfiguration.Create()
                    .EndpointName("TestSubscriber")
                    .ErrorQueueName("SubscriberErrors")
                    .AddMessageDefinition<TestMessage>(d => {
                        d.MessageAction = MessageAction.Event;
                        d.QueueName = "TestService";
                        d.HandlerType = typeof(TestMessageHandler);
                    });

            kernel.Bind<IMessageBusSerialiser>().To<JsonSerialiser>();
            kernel.Bind<IMessageBusConfiguration>().ToConstant(configuration).InSingletonScope();

            IMessageBusContainer container = new NinjectContainer(kernel);

            //var bus = new NSBMessageBus(container, configuration);
            //var bus = new RMQMessageBus(container, configuration);
            //var bus = new AMQMessageBus(container, configuration);
            var bus = new WSBMessageBus(container, configuration.HostName("campc-df7.bradyplc.com").ServiceNamespace("TheMagicBus"));

            bus.Start();

            Console.WriteLine("Press Enter to quit");
            Console.ReadLine();
        }
    }
}
