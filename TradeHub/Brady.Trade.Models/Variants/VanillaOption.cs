using Brady.Trade.Domain.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class VanillaOption : OptionDetails
    {
        public VanillaOption()
            : base("Vanilla")
        { }

        public decimal? CurrencyAmount { get; set; }
        public string CP { get; set; }
        public decimal? StrikePrice { get; set; }
        public string Model { get; set; }
        public DateTime? ExpiryMonth { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? PremiumDate { get; set; }
        public string PremiumCurrency { get; set; }
        public decimal? PremiumRate { get; set; }
        public decimal? PremiumAmount { get; set; }
    }
}
