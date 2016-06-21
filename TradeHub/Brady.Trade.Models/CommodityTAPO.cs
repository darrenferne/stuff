using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class CommodityTAPO : CommodityTrade, IAverageTrade, IOptionTrade
    {
        public CommodityTAPO()
            : base("CommodityTAPO")
        { }

        public IAverageDetails AverageDetails { get; set; }
        public IOptionDetails OptionDetails { get; set; }
    }
}
