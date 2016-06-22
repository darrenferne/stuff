using Brady.Trade.Domain.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class VanillaAverageDetails : AverageDetails
    {
        public VanillaAverageDetails()
            : base("VanillaAverageDetails")
        { }

        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual string FixingIndex { get; set; }
        public virtual decimal? AdditivePremium { get; set; }
        public virtual string AdditivePremiumUnits { get; set; }
        public virtual decimal? PercentagePremium { get; set; }
    }
}
