using Brady.Trade.DataService.Core.Interfaces;
using Brady.Trade.DataService.RecordTypes;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.Globalisation.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using FluentValidation;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.DataService
{
    public class TradeDataService : ConventionalDatabaseDataService<TradeDataService>, ITradeDataService
    {
        public TradeDataService(ITradeDataServiceSettings settings, ITradeDataServiceRepository repository, IHostConfiguration hostConfiguration, IEnumerable<IRecordType> recordTypes, IAuthorisation authorisation, IGlobalisationProvider globalisationProvider, IMetadataProvider metadataProvider, string databaseType = null, string connectionString = null)
            : base(settings.DataServiceName, globalisationProvider, repository as DatabaseDataServiceRepository, recordTypes, metadataProvider)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
        }

        public override IEnumerable<IRecordType> GetAdditionalRecordTypes()
        {
            //TODO - Refactor this when BWF-1452 is fixed
            var recordTypeAssembly = typeof(TradeRecordType).Assembly;
            var loadedRecordTypes = TradeDataServiceKernel.Kernel.GetAll<IRecordType>();
            var additionalRecordTypes = loadedRecordTypes.Where(rt => rt.GetType().Assembly == recordTypeAssembly);

            return base.GetAdditionalRecordTypes().Concat(additionalRecordTypes);
        }
    }
}
