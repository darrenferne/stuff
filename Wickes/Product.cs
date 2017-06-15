using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wickes
{
    public class Product : ComponentBase
    {
        public Product(string name = "", string company = "", string description = "")
            : base(name, description)
        {
            UpgradeCode = new Guid();
            Version = new Version();
            Culture = CultureInfo.CurrentCulture.Name;
            Features = new List<Feature>();

            Company = company;
        }

        public Guid UpgradeCode { get; set; }
        public string Company { get; set; }
        public string Culture { get; set; }
        public Version Version { get; set; }

        public List<Feature> Features { get; set; }
    }
}
