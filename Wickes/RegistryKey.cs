using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wickes
{
    public class RegistryKey : ComponentBase
    {
        public RegistryRoot Root { get; set; }
        public string Key { get; set; }

        public List<RegistryValue> Values { get; set; }
    }
}
