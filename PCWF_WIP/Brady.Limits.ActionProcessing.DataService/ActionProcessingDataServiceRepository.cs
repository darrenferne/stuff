using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.Globalisation.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ActionProcessingDataServiceRepository : ConventionalDatabaseDataServiceRepository, IActionProcessingDataServiceRepository
    {
        public ActionProcessingDataServiceRepository(
            IHostConfiguration hostConfiguration,
            IGlobalisationProvider globalisationProvider,
            IAuthorisation authorisation,
            IMetadataProvider metadataProvider,
            IRegisteredQueryManager registeredQueryManager)
            : base(hostConfiguration,
                globalisationProvider,
                authorisation,
                new List<string>(),
                metadataProvider,
                ActionProcessingDataServiceConstants.DataServiceName,
                ActionProcessingDataServiceConstants.SchemaName,
                registeredQueryManager: registeredQueryManager)
        { }
    }
}
