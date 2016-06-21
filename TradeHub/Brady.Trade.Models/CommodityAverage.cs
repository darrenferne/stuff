using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class CommodityAverage : CommodityTrade, IAverageTrade
    {
        public CommodityAverage()
            : base("CommodityAverage")
        { }

        public IAverageDetails AverageDetails { get; set; }
    }
}
