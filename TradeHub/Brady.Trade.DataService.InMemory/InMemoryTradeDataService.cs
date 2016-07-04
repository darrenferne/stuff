using Brady.Trade.DataService.Core.Interfaces;
using Brady.Trade.DataService.RecordTypes;
using Brady.Trade.Repository;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Interfaces;
using BWF.Globalisation.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using FluentValidation;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService.InMemory
{
    public class InMemoryTradeDataService : InMemoryDataService<InMemoryTradeDataService>, ITradeDataService
    {
        public InMemoryTradeDataService(ITradeDataServiceSettings settings, IInMemoryTradeDataServiceRepository repository, IHostConfiguration hostConfiguration, IEnumerable<IRecordType> recordTypes, IAuthorisation authorisation, IGlobalisationProvider globalisationProvider, IMetadataProvider metadataProvider, string databaseType = null, string connectionString = null)
            : base(settings.DataServiceName, globalisationProvider, repository as InMemoryDataServiceRepository, recordTypes, authorisation, metadataProvider)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
        }

        public override IEnumerable<IRecordType> GetAdditionalRecordTypes()
        {
            //TODO - Refactor this when BWF-1452 is fixed
            var recordTypeAssembly = typeof(TradeRecordType).Assembly;
            var loadedRecordTypes = InMemoryTradeDataServiceKernel.Kernel.GetAll<IRecordType>();
            var additionalRecordTypes = loadedRecordTypes.Where(rt => rt.GetType().Assembly == recordTypeAssembly);

            return base.GetAdditionalRecordTypes().Concat(additionalRecordTypes);
        }
    }
}
