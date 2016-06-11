using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core.Interfaces
{
    public interface IMessageBusContainer
    {
        T Resolve<T>();
    }
}
