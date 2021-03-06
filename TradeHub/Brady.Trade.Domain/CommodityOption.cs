﻿using Brady.Trade.Domain.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class CommodityOption : CommodityTrade, IOptionTrade
    {
        public CommodityOption()
            : base("CommodityOption")
        { }

        public virtual OptionDetails OptionDetails { get; set; }
       
    }
}
