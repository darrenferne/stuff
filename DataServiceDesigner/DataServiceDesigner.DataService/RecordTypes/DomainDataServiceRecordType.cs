using AutoMapper;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using System;
using System.Linq;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Create")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
    public class DataServiceRecordType : ChangeableRecordType<DomainDataService, long, DomainDataServiceBatchValidator>
    {
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
                        }
                    }
                    base.PreSaveAction(changeSet, context, username);
                };
            }
        }
    }
}
