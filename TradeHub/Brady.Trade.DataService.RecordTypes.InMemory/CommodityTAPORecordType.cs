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
    public class CommodityTAPORecordType : InMemoryRecordType<CommodityTAPO, CommodityTAPOValidator>
    {
        public CommodityTAPORecordType(IMetadataProvider metadataProvider, IInMemoryTradeDataServiceRepository repository) 
            : base(metadataProvider, repository)
        { }
    }
}
