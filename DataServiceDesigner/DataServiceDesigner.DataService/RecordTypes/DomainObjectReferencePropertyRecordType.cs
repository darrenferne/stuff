using AutoMapper;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Domain Object Reference Property")]
    [EditAction("Domain Object Reference Property")]
    [DeleteAction("Domain Object Reference Property")]
    public class DomainObjectReferencePropertyRecordType : ChangeableRecordType<DomainObjectReferenceProperty, long, DomainObjectReferencePropertyBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainObjectReferenceProperty, DomainObjectReferenceProperty>()
                .ForMember(m => m.ParentProperty, o => o.Ignore())
                .ForMember(m => m.ChildProperty, o => o.Ignore())
                .ForMember(m => m.Reference, o => o.Ignore());
        }
    }
}
