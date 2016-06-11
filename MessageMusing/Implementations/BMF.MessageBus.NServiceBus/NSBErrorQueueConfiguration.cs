using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.NServiceBus
{
    public class NSBErrorQueueConfiguration : IProvideConfiguration<MessageForwardingInCaseOfFaultConfig>
    {
        private string _queueName;

        public NSBErrorQueueConfiguration(string queueName)
        {
            _queueName = queueName;
        }

        public MessageForwardingInCaseOfFaultConfig GetConfiguration()
        {
            return new MessageForwardingInCaseOfFaultConfig
            {
                ErrorQueue = _queueName
            };
        }
    }
}
