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
    public class CommodityAverageMap : JoinedSubclassMapping<CommodityAverage>
    {
        public CommodityAverageMap()
        {
            Key(k =>
            {
                k.Column("TradeId");
                k.ForeignKey("fk_commodityaverage_commoditytrade");
                k.NotNullable(true);
                k.OnDelete(OnDeleteAction.Cascade);
            });

            ManyToOne(p => p.AverageDetails, m =>
            {
                m.Column("AverageDetailsId");
                m.NotNullable(true);
                m.ForeignKey("fk_commodityaverage_averagedetails");
                m.Cascade(Cascade.Persist);
                m.Lazy(LazyRelation.NoLazy);
            });
        }
    }
}
