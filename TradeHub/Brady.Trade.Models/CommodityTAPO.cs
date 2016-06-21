using Brady.Trade.Domain.BaseTypes;
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

        public AverageDetails AverageDetails { get; set; }
        public OptionDetails OptionDetails { get; set; }
    }
}
