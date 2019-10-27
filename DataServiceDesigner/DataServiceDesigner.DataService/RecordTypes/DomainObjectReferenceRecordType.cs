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
    public class DomainObjectReferenceRecordType : ChangeableRecordType<DomainObjectReference, long, DomainObjectReferenceBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainObjectReference, DomainObjectReference>()
                .ForMember(m => m.Schema, o => o.Ignore())
                .ForMember(m => m.Parent, o => o.Ignore())
                .ForMember(m => m.Child, o => o.Ignore())
                .ForMember(m => m.Properties, o => o.Ignore());
        }

        public override Action<ChangeSet<long, DomainObjectReference>, BatchSaveContext<long, DomainObjectReference>, string> PreSaveAction
        {
            get
            {
                return (changeSet, context, username) =>
                {
                    base.PreSaveAction(changeSet, context, username);
                };
            }
        }
    }
}
