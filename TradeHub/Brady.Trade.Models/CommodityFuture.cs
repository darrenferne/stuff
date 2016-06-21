﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Domain
{
    public class CommodityFuture : CommodityTrade
    {
        public CommodityFuture()
            : base("CommodityFuture")
        { }

        public decimal? CurrencyAmount { get; set; }
        public decimal? BasePrice { get; set; }
        public decimal? Spread { get; set; }
        public decimal? Price { get; set; }
    }
}
