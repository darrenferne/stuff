using BMF.MessageBus.Core;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDomain;

namespace TestSubscriber
{
    public class TestMessageHandler : MessageHandler<TestMessage>
    {
        public TestMessageHandler()
        { }

        public override void HandleMessage(TestMessage message)
        {
            Console.WriteLine($"Recieved a new Test message with id: {message.Id.ToString("N")}: {message.Message}");
        }
    }
}
