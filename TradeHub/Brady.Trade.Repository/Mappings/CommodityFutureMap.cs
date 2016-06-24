﻿using Brady.Trade.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Repository.Mappings
{
    public class CommodityFutureMap : JoinedSubclassMapping<CommodityFuture>
    {
        public CommodityFutureMap()
        {
            Key(k =>
            {
                k.Column("TradeId");
                k.ForeignKey("fk_commodityfuture_commoditytrade");
                k.NotNullable(true);
                k.OnDelete(OnDeleteAction.Cascade);
            });

            Property(t => t.BasePrice);
            Property(t => t.Spread);
            Property(t => t.Price);
            Property(t => t.CurrencyAmount);
        }
    }
}