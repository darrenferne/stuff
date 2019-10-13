using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Core.Interfaces.ChangeSets;
using BWF.DataServices.Core.Models;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Nancy.Interfaces;
using BWF.DataServices.PortableClients;
using DataServiceDesigner.Domain;
using SchemaBrowser.DataService;
using SchemaBrowser.Domain;
using System.Linq;
using System.Threading.Tasks;

namespace DataServiceDesigner.DataService
{
    public class DataServiceDesignedStartup : IStartUpTask
    {
        private readonly string BwfSystemUser = "_bwf_system";

        IDataServiceHostSettings _dshs;
        IAuthorisation _authorisation;
        IDataServiceDesignerDataService _dsdDataService;
        ISchemaBrowserConnectionManager _connectionManager;

        public DataServiceDesignedStartup(IDataServiceHostSettings dshs, IAuthorisation authorisation, IDataServiceDesignerDataService dsdDataService, ISchemaBrowserConnectionManager connectionManager)
        {
            _dshs = dshs;
            _authorisation = authorisation;
            _dsdDataService = dsdDataService;
            _connectionManager = connectionManager;
        }

        public long Position => 1;

        public Task<bool> Invoke()
        {
            return Task.Run(() =>
            {
                var builder = new QueryBuilder<DataServiceConnection>();
                var roleIds = _authorisation.GetAdministratorRoleIdsAsync().Result;
                var query = _dsdDataService.Query(new Query(builder.GetQuery()), BwfSystemUser, roleIds, string.Empty, _dshs.SystemToken, out var fault);

                if (!(query is null) && query.Records.Count > 0)
                {
                    var changeSet = new ChangeSet<long, DataServiceConnection>();
                    var itemRef = 1L;

                    foreach(var item in query.Records) 
                        changeSet.AddCreate(itemRef++, item, Enumerable.Empty<long>(), Enumerable.Empty<long>());

                    _connectionManager.SynchConnections(changeSet, _dshs.SystemToken);
                }

                return true;
            });
        }
    }
}
