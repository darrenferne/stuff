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
using DataServiceDesigner.Templating;
using System.IO;
using Nancy.Responses;
using System.Reflection;
using System.Threading.Tasks;

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

            Get[@"generatetemplate/{dataServiceName}", true] = async (args, ct) => {

                string dataServiceName = args.dataServiceName;

                var dataService = await GetDataService(dataServiceName);

                return Task.Run(() =>
                {
                    var templateGenerator = new TemplateGenerator();
                    var currentPath = Assembly.GetExecutingAssembly().Location;
                    var zipPath = Path.Combine(currentPath, $"DownLoads\\{dataServiceName}.zip");
                    templateGenerator.GenerateAllAndZip(dataService, zipPath);

                    var stream = new FileStream(zipPath, FileMode.Open);
                    var response = new StreamResponse(() => stream, MimeTypes.GetMimeType(zipPath));

                    return response.AsAttachment(zipPath);
                });
            };

            async Task<DomainDataService> GetDataService(string dataServiceName)
            {
                var builder = new QueryBuilder<DomainDataService>()
                .Expand(p => p.Connection)
                .Expand(p => p.Schemas)
                .Expand(p => p.Schemas[0].Objects)
                .Expand(p => p.Schemas[0].Objects[0].Properties)
                .Expand(p => p.Schemas[0].References)
                .Expand(p => p.Schemas[0].References[0].Properties)
                .Filter(d => d.Property(p => p.Name).EqualTo(dataServiceName));

                var roleIds = await _authorisation.GetAdministratorRoleIdsAsync();
                var queryResult = _dsdDataService.Query(new Query(builder.GetQuery()), BwfSystemUser, roleIds, string.Empty, _dshs.SystemToken, out var fault);

                var dataService = queryResult?.Records?.SingleOrDefault() as DomainDataService;
                return dataService;
            }
        }
    }
}
