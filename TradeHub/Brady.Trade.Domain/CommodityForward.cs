using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class CommodityForward : CommodityTrade
    {
        public CommodityForward()
            : base("CommodityForward")
        { }

        public virtual decimal? CurrencyAmount { get; set; }
        public virtual decimal? BasePrice { get; set; }
        public virtual decimal? Spread { get; set; }
        public virtual decimal? Price { get; set; }
    }
}
