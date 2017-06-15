using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enumbum.Tests
{
    [Flags]
    public enum TestFlagsEnum
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = One + Two,
        Four = 4,
        Five = One + Four,
        Six = Two + Four,
        Seven = Six + One,
        Eight = 8
    }
}
