using Brady.Trade.Domain;
using BWF.DataServices.Metadata.Fluent.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Metadata
{
    public class CommodityAverageSwapMetadata : TypeMetadataProvider<CommodityAverageSwap>
    {
        public CommodityAverageSwapMetadata()
        {
            Extends<CommodityTradeMetadata, Domain.CommodityTrade>();

            DisplayName("Commodity Average Swap");

            NumericProperty(x => x.StrikePrice)
                .DisplayName("Strike Price");
            
            NumericProperty(x => x.StrikeCurrencyAmount)
                .DisplayName("Strike Amount");

            TypeProperty(x => x.AverageDetails)
                .DisplayName("Average Details");

            ViewDefaults()
                .Property(x => x.StrikePrice)
                .Property(x => x.StrikeCurrencyAmount);
        }
    }
}
