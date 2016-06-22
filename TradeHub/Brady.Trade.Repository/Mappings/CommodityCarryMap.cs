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
    public class CommodityCarryMap : JoinedSubclassMapping<CommodityCarry>
    {
        public CommodityCarryMap()
        {
            Key(k =>
            {
                k.Column("TradeId");
                k.ForeignKey("fk_commoditycarry_commoditytrade");
                k.NotNullable(true);
                k.OnDelete(OnDeleteAction.Cascade);
            });
        }
    }
}
