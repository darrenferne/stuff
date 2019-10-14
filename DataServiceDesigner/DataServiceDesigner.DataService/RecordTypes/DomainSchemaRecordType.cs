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
    public class DomainSchemaRecordType : ChangeableRecordType<DomainSchema, long, DomainSchemaBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainSchema, DomainSchema>()
                .ForMember(m => m.DataService, o => o.Ignore())
                .ForMember(m => m.Objects, o => o.Ignore()); 
        }
        public override Action<ChangeSet<long, DomainSchema>, BatchSaveContext<long, DomainSchema>, string> PreSaveAction
        {
            get
            {
                return (changeSet, context, username) =>
                {
                    foreach (var schema in changeSet.Create.Values.Union(changeSet.Update.Values))
                    {
                        foreach (var obj in schema.Objects.Where(o => (o.Schema?.Id ?? 0) == 0))
                        {
                            obj.Schema = schema;
                        }
                    }
                    base.PreSaveAction(changeSet, context, username);

                };
            }
        }
    }
}
