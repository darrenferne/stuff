using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class VanillaAverage : IAverageDetails
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string FixingIndex { get; set; }
        public decimal? AdditivePremium { get; set; }
        public string AdditivePremiumUnits { get; set; }
        public decimal? PercentagePremium { get; set; }
        public bool? IsFixedPrice { get; set; }
        public decimal? FixedPrice { get; set; }
    }
}
