using AutoMapper;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Core.Models;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Nancy.Interfaces;
using BWF.DataServices.PortableClients;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using SchemaBrowser.DataService;
using SchemaBrowser.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Create")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
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
                .ForMember(x => x.Connection, m => m.Ignore())
                .ForMember(x => x.Schemas, m => m.Ignore());
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
                        }
                    }
                    base.PreSaveAction(changeSet, context, username);
                };
            }
        }
    }
}
