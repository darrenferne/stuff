using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.DataService.InMemory.Validation;
using Brady.Trade.Domain;
using Brady.Trade.Repository;
using BWF.DataServices.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.RecordTypes
{
    public class CommodityCarryRecordType : InMemoryRecordType<CommodityCarry, CommodityCarryValidator>
    {
        public CommodityCarryRecordType(IMetadataProvider metadataProvider, IInMemoryTradeDataServiceRepository repository) 
            : base(metadataProvider, repository)
        { }
    }
}
