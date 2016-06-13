using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMF.MessageBus.Core
{
    public enum MessageAction
    {
        Message = 0,
        Command = 1,
        Event = 2
    }
}
