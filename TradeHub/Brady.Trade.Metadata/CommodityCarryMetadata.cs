using Brady.Trade.Domain;
using BWF.DataServices.Metadata.Fluent.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Metadata
{
    public class CommodityCarryMetadata : TypeMetadataProvider<CommodityCarry>
    {
        public CommodityCarryMetadata()
        {
            Extends<CommodityTradeMetadata, Domain.CommodityTrade>();

            DisplayName("Commodity Carry");

            CollectionProperty(x => x.Legs)
                .DisplayName("Legs");
        }
    }
}
