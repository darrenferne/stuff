using Brady.Trade.DataService.Core.Interfaces;
using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.Globalisation.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
