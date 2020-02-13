using AutoMapper;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Metadata.Enums;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using System;
using System.Linq;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Data Service")]
    [EditAction("Data Service")]
    [DeleteAction("Data Service")]    
    public class DataServiceRecordType : ChangeableRecordType<DomainDataService, long, DomainDataServiceBatchValidator>
    {
        ISchemaBrowserHelpers _helpers;
        public DataServiceRecordType(ISchemaBrowserHelpers helpers)
        {
            _helpers = helpers;
        }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainDataService, DomainDataService>()
                .ForMember(m => m.Solution, o => o.Ignore())
                .ForMember(m => m.Connection, o => o.Ignore())
                .ForMember(m => m.Schemas, o => o.Ignore());
        }

        
        public override Action<ChangeSet<long, DomainDataService>, BatchSaveContext<long, DomainDataService>, string> PreSaveAction
        {
            get
            {
                return (changeSet, context, username) =>
                {
                    foreach (var dataService in changeSet.Create.Values.Union(changeSet.Update.Values))
                    {
                        foreach (var schema in dataService.Schemas.Where(s => (s.DataService?.Id ?? 0) == 0))
                        {
                            schema.DataService = dataService;

                            _helpers.AddDefaultObjectsToSchema(schema);
                            _helpers.AddDefaultRelationshipsToSchema(schema);
                        }
                    }
                    base.PreSaveAction(changeSet, context, username);
                };
            }
        }
    }
}
