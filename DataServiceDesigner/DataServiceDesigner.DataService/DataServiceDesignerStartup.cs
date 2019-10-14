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
    public class DataServiceDesignerStartup : IStartUpTask
    {
        IDataServiceDesignerRepository _dsdRepository;
        IDataServiceHostSettings _dshs;
        IAuthorisation _authorisation;
        IDataServiceDesignerDataService _dsdDataService;
        ISchemaBrowserHelpers _schemaBrowserHelpers;

        public DataServiceDesignerStartup(IDataServiceDesignerRepository dsdRepository, IDataServiceHostSettings dshs, IAuthorisation authorisation, IDataServiceDesignerDataService dsdDataService, ISchemaBrowserHelpers schemaBrowserHelpers)
        {
            _dsdRepository = dsdRepository;
            _dshs = dshs;
            _authorisation = authorisation;
            _dsdDataService = dsdDataService;
            _schemaBrowserHelpers = schemaBrowserHelpers;
        }

        public long Position => 1;

        public Task<bool> Invoke()
        {
            return Task.Run(() =>
            {
                var connections = _dsdRepository.GetAll<DataServiceConnection>().ToList();
                //var builder = new QueryBuilder<DataServiceConnection>();
                //var roleIds = _authorisation.GetAdministratorRoleIdsAsync().Result;
                //var query = _dsdDataService.Query(new Query(builder.GetQuery()), Constants.BwfSystemUser, roleIds, string.Empty, _dshs.SystemToken, out var fault);

                if (connections.Count > 0)
                {
                    var changeSet = new ChangeSet<long, DataServiceConnection>();
                    var itemRef = 1L;

                    foreach(var connection in connections) 
                        changeSet.AddCreate(itemRef++, connection, Enumerable.Empty<long>(), Enumerable.Empty<long>());

                    _schemaBrowserHelpers.SynchConnections(changeSet, _dshs.SystemToken);
                }

                return true;
            });
        }
    }
}
