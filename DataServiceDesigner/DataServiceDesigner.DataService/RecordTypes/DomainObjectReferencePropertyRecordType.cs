using AutoMapper;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Create")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
    public class DomainObjectReferencePropertyRecordType : ChangeableRecordType<DomainObjectReferenceProperty, long, DomainObjectReferencePropertyBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainObjectReferenceProperty, DomainObjectReferenceProperty>()
                .ForMember(m => m.Parent, o => o.Ignore())
                .ForMember(m => m.Child, o => o.Ignore())
                .ForMember(m => m.Reference, o => o.Ignore());
        }
    }
}
