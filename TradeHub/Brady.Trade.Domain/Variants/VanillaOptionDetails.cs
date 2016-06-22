using Brady.Trade.Domain.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class VanillaOptionDetails : OptionDetails
    {
        public VanillaOptionDetails()
            : base("VanillaOption")
        { }

        public virtual decimal? CurrencyAmount { get; set; }
        public virtual string CP { get; set; }
        public virtual decimal? StrikePrice { get; set; }
        public virtual string Model { get; set; }
        public virtual DateTime? ExpiryMonth { get; set; }
        public virtual DateTime? ExpiryDate { get; set; }
        public virtual DateTime? PremiumDate { get; set; }
        public virtual string PremiumCurrency { get; set; }
        public virtual decimal? PremiumRate { get; set; }
        public virtual decimal? PremiumAmount { get; set; }
    }
}
