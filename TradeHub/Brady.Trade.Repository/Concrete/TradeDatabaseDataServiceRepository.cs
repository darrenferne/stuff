using Brady.Trade.DataService.Core.Interfaces;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.Globalisation.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Mapping.ByCode;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Brady.Trade.Repository.Mappings;

namespace Brady.Trade.Repository.Concrete
{
    public class TradeDatabaseDataServiceRepository : ConventionalDatabaseDataServiceRepository, ICruddingTradeDataServiceRepository
    {
        //private string _databaseType;

        public TradeDatabaseDataServiceRepository(ITradeDataServiceSettings settings, IHostConfiguration hostConfiguration, IGlobalisationProvider globalisation, IAuthorisation authorisation, IMetadataProvider metadataProvider, string databaseType = null, string connectionString = null)
            : base(hostConfiguration, globalisation, authorisation, new List<string>(), metadataProvider, settings.DataServiceName, settings.DefaultSchema, databaseType, connectionString)
        { }
        
        protected override void SetMappingBeforeClass(ModelMapper mapper)
        {
            base.SetMappingBeforeClass(mapper);
            //need to add these in order
            mapper.AddMapping<TradeMap>();
            mapper.AddMapping<CommodityTradeMap>();
        }
    }
}