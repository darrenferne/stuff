using Brady.Limits.Dataservice.Interfaces;
using Brady.Limits.Selection.Provider;
using BWF.DataServices.Core.Interfaces;
using BWF.DataServices.Domain;
using BWF.DataServices.Nancy.Interfaces;
using BWF.DataServices.PortableClients;
using Nancy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bwfMetadata = BWF.DataServices.Metadata.TypeMetadata;

namespace Brady.Limits.DataService.Nancy.Modules
{
    public class SelectionModule : NancyModule
    {
        IDataServiceHostSettings _dshs;
        ILimitsRepository _repository;
        ISelectionQueryBuilder _queryBuilder;

        public SelectionModule(IDataServiceHostSettings dshs, ILimitsRepository limitsRepository)
            : base("ext/selection")
        {
            _dshs = dshs;
            _repository = limitsRepository;
            _queryBuilder = new SelectionQueryBuilder();

            Get["/{id}", true] = async (args, ct) =>
            {
                long selectionId = args.id;

                return await Task.Run(() =>
                {
                    var hostUrl = string.Empty;
                    var selection = _repository.Get<Domain.Models.Selection>(selectionId);
                    var query = _queryBuilder.GetQuery(selection);
                    var client = new DataServiceClient(hostUrl, LoginCredentials.FromToken(_dshs.SystemToken));
                    
                    var result = client.QueryAsync(query, selection.SourceSystem).Result;

                    return Response.AsJson(result);
                });
            };

        }
    }
}
