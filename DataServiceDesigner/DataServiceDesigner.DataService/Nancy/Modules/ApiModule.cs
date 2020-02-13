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
using System;

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
            : base("api/dataservicedesigner")
        {
            _dshs = dshs;
            _authorisation = authorisation;
            _dsdDataService = dsdDataService;
            _sbRepository = sbRepository;

            Get[@"/availableschemas/{dataServiceName}", true] = async (args, ct) =>
            {
                string dataServiceName = args.dataServiceName;

                var builder = new QueryBuilder<DomainDataService>()
                    .Filter(d => d.Property(p => p.Name).EqualTo(dataServiceName))
                    .Expand(d => d.Connection);

                var roleIds = await _authorisation.GetAdministratorRoleIdsAsync();
                var queryResult = _dsdDataService.Query(new Query(builder.GetQuery()), BwfSystemUser, roleIds, string.Empty, _dshs.SystemToken, out var fault);
                
                var dataService = queryResult?.Records?.SingleOrDefault() as DomainDataService;

                if (dataService is null)
                    return Response.AsJson(Enumerable.Empty<DbSchema>(), HttpStatusCode.BadRequest);

                var schemas = _sbRepository.GetWhere<DbSchema>(s => s.Connection.Name == dataService.Connection.Name);
                return Response.AsJson(schemas);
            };

            Get[@"generatetemplate/{solutionName}", true] = async (args, ct) => {

                string solutionName = args.solutionName;

                var solution = await GetSolution(solutionName);

                return await Task.Run(() =>
                {
                    var templateGenerator = new TemplateGenerator();
                    var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    var zipPath = Path.Combine(baseDir, $"DownLoads\\{solutionName}");
                    var zipFile = templateGenerator.GenerateAllAndZip(solution, zipPath);

                    var stream = new FileStream(zipFile, FileMode.Open);
                    var response = new StreamResponse(() => stream, MimeTypes.GetMimeType(zipFile));
                    var fileName = Path.GetFileName(zipFile);
                    var attachment = response.AsAttachment(fileName);
                    return attachment;
                });
            };

            async Task<DataServiceSolution> GetSolution(string solutionName)
            {
                var builder = new QueryBuilder<DataServiceSolution>()
                    .Expand(p => p.DataServices[0].Solution)
                    .Expand(p => p.DataServices[0].Connection)
                    .Expand(p => p.DataServices[0].Schemas)
                    .Expand(p => p.DataServices[0].Schemas[0].Objects)
                    .Expand(p => p.DataServices[0].Schemas[0].Objects[0].Properties)
                    .Expand(p => p.DataServices[0].Schemas[0].References)
                    .Expand(p => p.DataServices[0].Schemas[0].References[0].Properties)
                    .Filter(d => d.Property(p => p.Name).EqualTo(solutionName));

                var roleIds = await _authorisation.GetAdministratorRoleIdsAsync();
                var queryResult = _dsdDataService.Query(new Query(builder.GetQuery()), BwfSystemUser, roleIds, string.Empty, _dshs.SystemToken, out var fault);

                var solution = queryResult?.Records?.SingleOrDefault() as DataServiceSolution;
                return solution;
            }
        }
    }
}
