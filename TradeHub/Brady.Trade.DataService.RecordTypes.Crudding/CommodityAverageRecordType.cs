using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.DataService.Database.Validation;
using Brady.Trade.Domain;
using BWF.DataServices.Core.Interfaces;

namespace Brady.Trade.DataService.RecordTypes
{
    public class CommodityAverageRecordType : ChangeableTradeRecordType<CommodityAverage, CommodityAverageValidator, CommodityAverageDeleteValidator>
    {
        public CommodityAverageRecordType(IMetadataProvider metadataProvider) 
            : base(metadataProvider)
        {
        }
    }
}
