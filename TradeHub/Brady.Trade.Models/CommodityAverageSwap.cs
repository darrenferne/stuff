using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class CommodityAverageSwap : CommodityTrade, IAverageTrade
    {
        public CommodityAverageSwap()
            : base("CommodityAverageSwap")
        { }

        public double StrikePrice { get; set; }
        public double StrikeCurrencyAmount { get; set; }
        public IAverageDetails AverageDetails { get; set; }
    }
}
