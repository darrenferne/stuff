using Brady.Limits.Domain;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.Globalisation.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using System.Collections.Generic;

namespace Brady.Limits.DataService
{
    public class LimitsDataServiceRepository : ConventionalDatabaseDataServiceRepository, ILimitsDataServiceRepository
    {
        public LimitsDataServiceRepository(IHostConfiguration hostConfiguration, IGlobalisationProvider globalisation, IAuthorisation authorisation, IMetadataProvider metadataProvider, string databaseType = null, string connectionString = null)
            : base(hostConfiguration, globalisation, authorisation, new List<string>(), metadataProvider, Constants.DataServiceName, Constants.DefaultDatabaseSchema, databaseType, connectionString)
        { }
    }
}
