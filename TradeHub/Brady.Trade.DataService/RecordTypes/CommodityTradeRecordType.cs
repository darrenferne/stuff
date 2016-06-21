using Brady.Trade.DataService.Abstract;
using Brady.Trade.DataService.Validators;
using BWF.DataServices.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.RecordTypes
{
    public class CommodityTradeRecordType : ChangeableTradeRecordType<Domain.CommodityTrade, CommodityTradeValidator, CommodityTradeDeleteValidator>
    {
        public CommodityTradeRecordType(IMetadataProvider metadataProvider) 
            : base(metadataProvider)
        {
        }
    }
}
