using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.Globalisation.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using System.Collections.Generic;
using DataServiceDesigner.Domain;
using NHibernate;

namespace DataServiceDesigner.DataService
{
    public class DataServiceDesignerRepository : ConventionalDatabaseDataServiceRepository, IDataServiceDesignerRepository
    {
        ISessionFactory _internalFactory;
        public DataServiceDesignerRepository(
           IHostConfiguration hostConfiguration,
           IGlobalisationProvider globalisationProvider,
           IAuthorisation authorisation,
           IMetadataProvider metadataProvider)
        : base(
            hostConfiguration,
            globalisationProvider,
            authorisation,
            new List<string>(),
            metadataProvider,
            Constants.DataServiceName,
            Constants.DataServiceSchema) 
        {
            
        }

        //internal ISession GetInternalSession()
        //{
        //    if (_internalFactory == null)
        //        _internalFactory = base.internalCreateSessionFactory(this.DatabaseType, this.ConnectionString);

        //    return _internalFactory.OpenSession();
        //}
        //public DesignerConnection GetConnection(long id)
        //{
        //    using (var session = GetInternalSession())
        //    {
        //        return session.Get<DesignerConnection>(id);
        //    }
        //}

        //public DesignerDataService GetDataService(long id)
        //{
        //    using (var session = GetInternalSession())
        //    {
        //        return session.Get<DesignerDataService>(id);
        //    }
        //}
    }
}
