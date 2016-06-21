using Brady.Trade.Domain;
using BWF.DataServices.Metadata.Fluent.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Metadata
{
    public class CommodityTradeMetadata : TypeMetadataProvider<CommodityTrade>
    {
        public CommodityTradeMetadata()
        {
            Extends<TradeMetadata, Domain.Trade>();

            DisplayName("Commodity Trade");

            StringProperty(x => x.BS)
                .DisplayName("BS");

            StringProperty(x => x.Term)
                .DisplayName("Term");

            DateProperty(x => x.DeliveryMonth)
                .DisplayName("Delivery Month");

            DateProperty(x => x.DeliveryDate)
                .DisplayName("Delivery Date");

            NumericProperty(x => x.Lots)
                .DisplayName("Lots");

            NumericProperty(x => x.CommodityAmount)
                .DisplayName("Commodity Amount");

            StringProperty(x => x.CommodityUnits)
                .DisplayName("Commodity Units");

            ViewDefaults()
                .Property(x => x.BS)
                .Property(x => x.Term)
                .Property(x => x.DeliveryMonth)
                .Property(x => x.DeliveryDate)
                .Property(x => x.Lots)
                .Property(x => x.CommodityAmount)
                .Property(x => x.CommodityUnits);
        }
    }
}
