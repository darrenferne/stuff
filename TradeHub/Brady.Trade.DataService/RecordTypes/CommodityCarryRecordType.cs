using Brady.Trade.DataService.Abstract;
using Brady.Trade.DataService.Validators;
using Brady.Trade.Domain;
using BWF.DataServices.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.RecordTypes
{
    public class CommodityCarryRecordType : ChangeableTradeRecordType<CommodityCarry, CommodityCarryValidator, CommodityCarryDeleteValidator>
    {
        public CommodityCarryRecordType(IMetadataProvider metadataProvider) 
            : base(metadataProvider)
        {
        }
    }
}
