using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core.Interfaces
{
    public interface IMessageBus : IDisposable
    {
        void Start();
        void Stop();

        void Send<T_message>(string destination, T_message message);
        void Publish<T_message>(T_message message);

        void Subscribe(Type messageType);
        void Subscribe<T_message>();

        void Unsubscribe(Type messageType);
        void Unsubscribe<T_message>();
    }
}
