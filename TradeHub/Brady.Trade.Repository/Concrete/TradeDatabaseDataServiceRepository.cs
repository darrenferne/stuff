using Brady.Trade.Repository.Interfaces;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.Globalisation.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Trade.Repository.Concrete
{
    public class TradeDatabaseDataServiceRepository : ConventionalDatabaseDataServiceRepository, ITradeDataServiceRepository
    {
        public TradeDatabaseDataServiceRepository(IHostConfiguration hostConfiguration, IGlobalisationProvider globalisation, IAuthorisation authorisation, IMetadataProvider metadataProvider, string dataServiceName, string databaseType = null, string connectionString = null)
            : base(hostConfiguration, globalisation, authorisation, new List<string>(), metadataProvider, dataServiceName, "brady_trading", databaseType, connectionString)
        { }
    }
}
