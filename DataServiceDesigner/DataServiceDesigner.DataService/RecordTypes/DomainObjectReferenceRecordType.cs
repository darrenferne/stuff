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
                .ForMember(x => x.Parent, m => m.Ignore())
                .ForMember(x => x.Child, m => m.Ignore())
                .ForMember(x => x.Properties, m => m.Ignore());
        }
    }
}
