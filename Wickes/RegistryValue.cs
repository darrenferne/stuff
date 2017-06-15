using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wickes
{
    public class RegistryValue : ComponentBase
    {
        public RegistryRoot Root { get; set; }
        public string Key { get; set; }
        public RegistryValueType Type { get; set; }
        public object Value { get; set; }
    }
}
