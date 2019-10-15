using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Nancy.Abstract;
using Nancy;
using SchemaBrowser.Domain;
using SchemaBrowser.DataService;
using DataServiceDesigner.Domain;
using BWF.DataServices.PortableClients;
using System.Linq;
using BWF.DataServices.Nancy.Interfaces;
using BWF.DataServices.Core.Models;

namespace DataServiceDesigner.DataService
{
    public class ApiModule : BWFNancyModule
    {
        readonly string BwfSystemUser = "_bwf_system";

        IDataServiceHostSettings _dshs;
        IAuthorisation _authorisation;
        IDataServiceDesignerDataService _dsdDataService;
        ISchemaBrowserRepository _sbRepository;
        
        public ApiModule(IDataServiceHostSettings dshs, IAuthorisation authorisation, IDataServiceDesignerDataService dsdDataService, ISchemaBrowserRepository sbRepository)
            : base("api/dataServicedesigner")
        {
            _dshs = dshs;
            _authorisation = authorisation;
            _dsdDataService = dsdDataService;
            _sbRepository = sbRepository;

            Get[@"/availableschemas/{dataServiceName}", true] = async (args, ct) =>
            {
                string dataServiceName = args.dataServiceName;

                var builder = new QueryBuilder<DomainDataService>()
                    .Filter(d => d.Property(p => p.Name).EqualTo(dataServiceName));
                    //.Expand(d => d.Connection);

                var roleIds = await _authorisation.GetAdministratorRoleIdsAsync();
                var queryResult = _dsdDataService.Query(new Query(builder.GetQuery()), BwfSystemUser, roleIds, string.Empty, _dshs.SystemToken, out var fault);
                
                var dataService = queryResult?.Records?.SingleOrDefault() as DomainDataService;

                if (dataService is null)
                    return Response.AsJson(Enumerable.Empty<DbSchema>(), HttpStatusCode.BadRequest);

                var schemas = _sbRepository.GetWhere<DbSchema>(s => s.Connection.Name == dataService.Connection.Name);
                return Response.AsJson(schemas);
            };
        }
    }
}
