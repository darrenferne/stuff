﻿using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.DataService.Core.Validators;
using Brady.Trade.Domain;
using BWF.DataServices.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.RecordTypes
{
    public class CommodityOptionRecordType : ChangeableTradeRecordType<CommodityOption, CommodityOptionValidator, CommodityOptionDeleteValidator>
    {
        public CommodityOptionRecordType(IMetadataProvider metadataProvider) 
            : base(metadataProvider)
        {
        }
    }
}