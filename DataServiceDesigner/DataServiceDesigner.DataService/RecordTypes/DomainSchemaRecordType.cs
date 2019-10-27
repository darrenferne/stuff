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
    [CreateAction("Domain Schema")]
    [EditAction("Domain Schema")]
    [DeleteAction("Domain Schema")]
    public class DomainSchemaRecordType : ChangeableRecordType<DomainSchema, long, DomainSchemaBatchValidator>
    {
        ISchemaBrowserHelpers _helpers;
        public DomainSchemaRecordType(ISchemaBrowserHelpers helpers)
        {
            _helpers = helpers;
        }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainSchema, DomainSchema>()
                .ForMember(m => m.DataService, o => o.Ignore())
                .ForMember(m => m.Objects, o => o.Ignore())
                .ForMember(m => m.References, o => o.Ignore()); 
        }
        public override Action<ChangeSet<long, DomainSchema>, BatchSaveContext<long, DomainSchema>, string> PreSaveAction
        {
            get
            {
                return (changeSet, context, username) =>
                {
                    foreach (var domainSchema in changeSet.Create.Values.Union(changeSet.Update.Values))
                    {
                        foreach (var domainObject in domainSchema.Objects.Where(o => (o.Schema?.Id ?? 0) == 0))
                        {
                            domainObject.Schema = domainSchema;

                            _helpers.AddDefaultPropertiesToObject(domainObject);
                        }
                    }

                    base.PreSaveAction(changeSet, context, username);

                };
            }
        }
    }
}
