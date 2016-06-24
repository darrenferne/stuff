﻿using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.DataService.Core.Validators;
using Brady.Trade.Domain;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Metadata.Attributes.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.RecordTypes
{
    [CreateAction("Commodity Trade", IncludeSubTypes = new string[] { "CommodityFuture", "CommodityForward", "CommodityAverage", "CommodityAverageSwap", "CommodityOption", "CommodityTAPO", "CommodityCarry" },
                                     SubTypeDisplayNames = new string[] { "Commodity Future", "Commodity Forward", "Commodity Average", "Commodity Average Swap", "Commodity Option", "Commodity TAPO", "Commodity Carry" })]
    [EditAction("Commodity Trade")]
    [DeleteAction("Commodity Trade")]
    public class CommodityTradeRecordType : ChangeableTradeRecordType<Domain.CommodityTrade, CommodityTradeValidator, CommodityTradeDeleteValidator>
    {
        public CommodityTradeRecordType(IMetadataProvider metadataProvider) 
            : base(metadataProvider)
        {
        }

        public override void ConfigureMapper()
        {
            AutoMapper.Mapper.CreateMap<CommodityTrade, CommodityTrade>()
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