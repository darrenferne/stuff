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

        public NSBErrorQueueConfiguration()
        {
            _queueName = string.IsNullOrEmpty(NSBGlobalConfiguration.Config.ErrorQueueName) ? "Errors" : NSBGlobalConfiguration.Config.ErrorQueueName;
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
