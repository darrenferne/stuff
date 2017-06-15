using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wickes
{
    public class Directory : ComponentBase
    {
        public Directory Parent { get; set; }
        public string Path { get; set; }
    }
}
