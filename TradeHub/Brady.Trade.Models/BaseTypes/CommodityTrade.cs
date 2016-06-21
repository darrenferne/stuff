using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public abstract class CommodityTrade : Trade
    {
        public CommodityTrade(string tradeType)
            : base(tradeType)
        { }

        public string BS { get; set; }
        public string Term { get; set; }
        public DateTime? DeliveryMonth { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public decimal? Lots { get; set; }
        public decimal? CommodityAmount { get; set; }
        public string CommodityUnits { get; set; }
    }
}
