using Brady.Trade.DataService.Core.Abstract;
using Brady.Trade.DataService.InMemory.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BWF.DataServices.Core.Interfaces;
using Brady.Trade.Domain;
using Brady.Trade.Repository;

namespace Brady.Trade.DataService.RecordTypes
{
    public class VanillaAverageDetailsRecordType : AverageDetailsRecordType<VanillaAverageDetails>
    {
        public VanillaAverageDetailsRecordType(IMetadataProvider metadataProvider, IInMemoryTradeDataServiceRepository repository) 
            : base(metadataProvider, repository)
        {
        }
    }
}
