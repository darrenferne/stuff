using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core.Interfaces
{
    public interface IMessageBus
    {
        void Publish<T_message>(T_message message);
        void Subscribe<T_message>();
        int Subscribe<T_message>(Action<T_message> action);
        void Unsubscribe<T_message>();
        void Unsubscribe<T_message>(int subscription);

        //void Send<T_message, T_route>(T_message message, T_route route);
        //void Recieve<T_message>(Action<T_message> action);

    }
}
