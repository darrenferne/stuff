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
    public class CommodityOptionMap : JoinedSubclassMapping<CommodityOption>
    {
        public CommodityOptionMap()
        {
            Key(k =>
            {
                k.Column("TradeId");
                k.ForeignKey("fk_commodityoption_commoditytrade");
                k.NotNullable(true);
                k.OnDelete(OnDeleteAction.Cascade);
            });

            ManyToOne(p => p.OptionDetails, m =>
            {
                m.Column("OptionDetailsId");
                m.NotNullable(true);
                m.ForeignKey("fk_commodityoption_optiondetails");
                m.Cascade(Cascade.Persist);
                m.Lazy(LazyRelation.NoLazy);
            });
        }
    }
}
