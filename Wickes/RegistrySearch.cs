using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wickes
{
    public class RegistrySearch : Search
    {
        public RegistryRoot Root { get; set; }
        public string Key { get; set; }
        public RegistryValueType Type { get; set; }
    }
}
