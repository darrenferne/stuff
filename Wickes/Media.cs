using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wickes
{
    public class Media : ComponentBase
    {
        public int MediaNumber { get; set; }
        public string MediaPrompt { get; set; }
        public string CabinetName { get; set; }
        public bool EmbedCabinet { get; set; }
    }
}
