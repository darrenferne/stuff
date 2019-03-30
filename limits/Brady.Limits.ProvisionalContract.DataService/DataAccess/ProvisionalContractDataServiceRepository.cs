using Brady.Limits.ProvisionalContract.Domain;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.Globalisation.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brady.Limits.ProvisionalContract.DataService
{
    public class ProvisionalContractDataServiceRepository : ConventionalDatabaseDataServiceRepository, IProvisionalContractDataServiceRepository
    {
        public ProvisionalContractDataServiceRepository(IHostConfiguration hostConfiguration, IGlobalisationProvider globalisation, IAuthorisation authorisation, IMetadataProvider metadataProvider, string databaseType = null, string connectionString = null)
            : base(hostConfiguration, globalisation, authorisation, new List<string>(), metadataProvider, Constants.DataServiceName, Constants.DefaultDatabaseSchema, databaseType, connectionString)
        { }
    }
}
