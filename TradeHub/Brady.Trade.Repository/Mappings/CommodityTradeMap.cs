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
    public class CommodityTradeMap : JoinedSubclassMapping<CommodityTrade>
    {
        public CommodityTradeMap()
        {
            Key(k =>
            {
                k.Column("TradeId");
                k.ForeignKey("fk_commoditytrade_trade");
                k.NotNullable(true);
                k.OnDelete(OnDeleteAction.Cascade);
            });

            Property(t => t.BS);
            Property(t => t.CommodityAmount);
            Property(t => t.CommodityUnits);
            Property(t => t.DeliveryDate);
            Property(t => t.DeliveryMonth);
            Property(t => t.Lots);
            Property(t => t.Term);
        }
    }
}
