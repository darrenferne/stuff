using Brady.Trade.Domain.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class VanillaAverage : AverageDetails
    {
        public VanillaAverage()
            : base("Vanilla")
        { }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string FixingIndex { get; set; }
        public decimal? AdditivePremium { get; set; }
        public string AdditivePremiumUnits { get; set; }
        public decimal? PercentagePremium { get; set; }
    }
}
