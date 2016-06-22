using Brady.Trade.DataService.Core.Interfaces;
using BWF.DataServices.Metadata.Fluent.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Metadata
{
    public class TradeMetadataBundle : TypeMetadataBundle
    {
        public TradeMetadataBundle(ITradeDataServiceSettings settings)
            : base(settings.DataServiceName, 
                new TradeMetadata(),
                new CommodityTradeMetadata(),
                new CommodityFutureMetadata(),
                new CommodityForwardMetadata(),
                new AverageDetailsMetadata(),
                new VanillaAverageDetailsMetadata(),
                new OptionDetailsMetadata(),
                new VanillaOptionDetailsMetadata(),
                new CommodityAverageMetadata(),
                new CommodityAverageSwapMetadata(),
                new CommodityOptionMetadata(),
                new CommodityTAPOMetadata(),
                new CommodityCarryMetadata())
        {
        }
    }
}
