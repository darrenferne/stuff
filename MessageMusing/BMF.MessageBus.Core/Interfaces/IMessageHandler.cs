using BMF.MessageBus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core
{
    public interface IMessageHandler<T_message> : IMessageHandler
    {
        void HandleMessage(T_message message);
    }

    public interface IMessageHandler
    {
        IMessageBus Bus { get; set; }
        Type MessageType { get; }
    }
}
