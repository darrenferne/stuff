using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.DataService.Core.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Metadata.Attributes.Actions;
using Brady.Trade.Domain;

namespace Brady.Trade.DataService.RecordTypes
{
    [CreateAction("Trade", IncludeSubTypes = new string[] { "CommodityFuture", "CommodityForward", "CommodityAverage", "CommodityAverageSwap", "CommodityOption", "CommodityTAPO", "CommodityCarry" },
                           SubTypeDisplayNames = new string[] { "Commodity Future", "Commodity Forward", "Commodity Average", "Commodity Average Swap", "Commodity Option", "Commodity TAPO", "Commodity Carry" })]
    [EditAction("Trade")]
    [DeleteAction("Trade")]
    public class TradeRecordType : ChangeableTradeRecordType<Domain.Trade, TradeValidator, TradeDeleteValidator>
    {
        public TradeRecordType(IMetadataProvider metadataProvider) 
            : base(metadataProvider)
        {
        }

        public override void ConfigureMapper()
        {
            AutoMapper.Mapper.CreateMap<Domain.Trade, Domain.Trade>()
                .Include<CommodityTrade, CommodityTrade>()
                .Include<CommodityFuture, CommodityFuture>()
                .Include<CommodityForward, CommodityForward>()
                .Include<CommodityAverage, CommodityAverage>()
                .Include<CommodityAverageSwap, CommodityAverageSwap>()
                .Include<CommodityOption, CommodityOption>()
                .Include<CommodityTAPO, CommodityTAPO>()
                .Include<CommodityCarry, CommodityCarry>();
        }
    }
}
