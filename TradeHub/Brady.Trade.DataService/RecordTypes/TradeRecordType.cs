﻿using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.DataService.Core.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BWF.DataServices.Core.Interfaces;

namespace Brady.Trade.DataService.RecordTypes
{
    public class TradeRecordType : ChangeableTradeRecordType<Domain.Trade, TradeValidator, TradeDeleteValidator>
    {
        public TradeRecordType(IMetadataProvider metadataProvider) 
            : base(metadataProvider)
        {
        }
    }
}
