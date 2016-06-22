using Brady.Trade.Domain.BaseTypes;
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

        public virtual double StrikePrice { get; set; }
        public virtual double StrikeCurrencyAmount { get; set; }
        public virtual AverageDetails AverageDetails { get; set; }
    }
}
