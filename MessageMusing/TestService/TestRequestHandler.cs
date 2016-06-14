using BMF.MessageBus.Core;
using BMF.MessageBus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDomain;

namespace TestService
{
    public class TestRequestHandler : MessageHandler<TestRequest>
    {
        public TestRequestHandler()
        { }

        public override void HandleMessage(TestRequest request)
        {
            Console.WriteLine($"Recieved a new Test Request message with id: { request.Id.ToString("N") }");
            if (Bus != null)
            {
                Console.WriteLine($"Publishing {request.MessageCount} new Test messages");
                var sw = Stopwatch.StartNew();
                for (int x = 0; x < request.MessageCount; x++)
                {
                    var id = Guid.NewGuid();
                    var message = new TestMessage
                    {
                        Id = Guid.NewGuid(),
                        Message = "Hello World".PadRight(1000) + "Ha!"
                    };

                    Bus.Publish(message);
                    Console.WriteLine($"Published a new Test message with id: {id.ToString("N")}");
                }
                sw.Stop();
                Console.WriteLine($"Published {request.MessageCount} new Test messages in {sw.ElapsedMilliseconds}ms");
            }
        }
    }
}
