using BWF.DataServices.Core.Abstract;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.Globalisation.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ActionProcessingDataService : ConventionalDatabaseDataService<ActionProcessingDataService>, IActionProcessingDataService
    {
        public ActionProcessingDataService(
            IHostConfiguration hostConfiguration,
            IEnumerable<IRecordType> recordTypes,
            IGlobalisationProvider globalisationProvider,
            IAuthorisation authorisation,
            IMetadataProvider metadataProvider,
            IActionProcessingDataServiceRepository actionProcessingDataServiceRepository,
            IDatabaseStreamingQueryExecutor databaseStreamingQueryExecutor)
        : base(ActionProcessingDataServiceConstants.DataServiceName,
            globalisationProvider,
            actionProcessingDataServiceRepository as DatabaseDataServiceRepository,
            recordTypes,
            metadataProvider,
            databaseStreamingQueryExecutor)
        { }
    }
}
