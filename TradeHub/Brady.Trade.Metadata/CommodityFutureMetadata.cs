using Brady.Trade.Domain;
using BWF.DataServices.Metadata.Fluent.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Metadata
{
    public class CommodityFutureMetadata : TypeMetadataProvider<CommodityFuture>
    {
        public CommodityFutureMetadata()
        {
            Extends<CommodityTradeMetadata, Domain.CommodityTrade>();

            DisplayName("Commodity Future");
            
            NumericProperty(x => x.CurrencyAmount)
                .DisplayName("Currency Amount");

            NumericProperty(x => x.BasePrice)
                .DisplayName("Base Price");

            NumericProperty(x => x.Spread)
                .DisplayName("Spread");

            NumericProperty(x => x.Price)
                .DisplayName("Price");
            
            ViewDefaults()
                .Property(x => x.CurrencyAmount)
                .Property(x => x.BasePrice)
                .Property(x => x.Spread)
                .Property(x => x.Price);
        }
    }
}
