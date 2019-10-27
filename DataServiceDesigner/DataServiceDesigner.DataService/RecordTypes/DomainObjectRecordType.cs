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
    [CreateAction("Domain Object")]
    [EditAction("Domain Object")]
    [DeleteAction("Domain Object")]
    public class DomainObjectRecordType : ChangeableRecordType<DomainObject, long, DomainObjectBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainObject, DomainObject>()
                .ForMember(x => x.Schema, m => m.Ignore())
                .ForMember(x => x.Properties, m => m.Ignore());
        }

        public override Action<ChangeSet<long, DomainObject>, BatchSaveContext<long, DomainObject>, string> PreSaveAction
        {
            get
            {
                return (changeSet, context, username) =>
                {
                    foreach (var domainObject in changeSet.Create.Values.Union(changeSet.Update.Values))
                    {
                        foreach (var domainProperty in domainObject.Properties.Where(o => (o.Object?.Id ?? 0) == 0))
                        {
                            domainProperty.Object = domainObject;
                        }
                    }

                    base.PreSaveAction(changeSet, context, username);
                };
            }
        }
    }
}
