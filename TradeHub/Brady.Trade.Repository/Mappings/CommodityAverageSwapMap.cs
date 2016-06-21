using Brady.Trade.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Repository.Mappings
{
    public class CommodityAverageSwapMap : JoinedSubclassMapping<CommodityAverageSwap>
    {
        public CommodityAverageSwapMap()
        {
            Key(k =>
            {
                k.Column("TradeId");
                k.ForeignKey("fk_commodityaverageswap_commoditytrade");
                k.NotNullable(true);
                k.OnDelete(OnDeleteAction.Cascade);
            });

            Property(cas => cas.StrikePrice);

            Property(cas => cas.StrikeCurrencyAmount);

            ManyToOne(p => p.AverageDetails, m =>
            {
                m.Column("AverageDetailsId");
                m.NotNullable(true);
                m.ForeignKey("fk_commodityaverageswap_averagedetails");
                m.Cascade(Cascade.Persist);
                m.Lazy(LazyRelation.NoLazy);
            });
        }
    }
}
