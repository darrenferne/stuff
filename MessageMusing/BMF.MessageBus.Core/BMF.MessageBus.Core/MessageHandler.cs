﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core
{
    public abstract class MessageHandler<T_message> : IMessageHandler<T_message>
    {
        private Type _messageType = typeof(T_message);

        public Type MessageType { get { return _messageType; } }

        public abstract void HandleMessage(T_message message);
    }
}
