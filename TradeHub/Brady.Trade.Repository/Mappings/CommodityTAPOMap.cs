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
    public class CommodityTAPOMap : JoinedSubclassMapping<CommodityTAPO>
    {
        public CommodityTAPOMap()
        {
            Key(k =>
            {
                k.Column("TradeId");
                k.ForeignKey("fk_commoditytapo_commoditytrade");
                k.NotNullable(true);
                k.OnDelete(OnDeleteAction.Cascade);
            });

            ManyToOne(p => p.AverageDetails, m =>
            {
                m.Column("AverageDetailsId");
                m.NotNullable(true);
                m.ForeignKey("fk_commoditytapo_averagedetails");
                m.Cascade(Cascade.Persist);
                m.Lazy(LazyRelation.NoLazy);
            });

            ManyToOne(p => p.OptionDetails, m =>
            {
                m.Column("OptionDetailsId");
                m.NotNullable(true);
                m.ForeignKey("fk_commoditytapo_optiondetails");
                m.Cascade(Cascade.Persist);
                m.Lazy(LazyRelation.NoLazy);
            });
        }
    }
}
