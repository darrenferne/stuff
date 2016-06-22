using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public abstract class CommodityTrade : Trade
    {
        internal CommodityTrade()
            : base("CommodityTrade")
        { }

        internal CommodityTrade(string tradeType)
            : base(tradeType)
        { }

        public virtual string BS { get; set; }
        public virtual string Term { get; set; }
        public virtual DateTime? DeliveryMonth { get; set; }
        public virtual DateTime? DeliveryDate { get; set; }
        public virtual decimal? Lots { get; set; }
        public virtual decimal? CommodityAmount { get; set; }
        public virtual string CommodityUnits { get; set; }
    }
}
