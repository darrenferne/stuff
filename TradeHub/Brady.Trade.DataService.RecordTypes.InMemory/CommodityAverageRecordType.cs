using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.DataService.InMemory.Validation;
using Brady.Trade.Domain;
using Brady.Trade.Repository;
using BWF.DataServices.Core.Interfaces;

namespace Brady.Trade.DataService.RecordTypes
{
    public class CommodityAverageRecordType : InMemoryRecordType<CommodityAverage, CommodityAverageValidator>
    {
        public CommodityAverageRecordType(IMetadataProvider metadataProvider, IInMemoryTradeDataServiceRepository repository) 
            : base(metadataProvider, repository)
        { }
    }
}
