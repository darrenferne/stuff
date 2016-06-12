﻿using BMF.MessageBus.Core;
using BMF.MessageBus.Core.Interfaces;
using BMF.MessageBus.NServiceBus;
using BMF.MessageBus.Serialisers;
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
            IMessageBusConfiguration configuration = new MessageBusConfiguration("TestSubScriber", "SubscriberErrors", new MessageMetadata<TestMessage>() { MessageAction = MessageAction.Event, QueueName = "TestService", HandlerType = typeof(TestMessageHandler) });

            kernel.Bind<IMessageBusSerialiser>().To<JsonSerialiser>();
            kernel.Bind<IMessageBusConfiguration>().ToConstant(configuration).InSingletonScope();

            IMessageBusContainer container = new NinjectContainer(kernel);

            var bus = new NSBMessageBus(container, configuration);

            bus.Start();

            Console.Write("Press Enter to quit");
            Console.ReadLine();
        }
    }
}