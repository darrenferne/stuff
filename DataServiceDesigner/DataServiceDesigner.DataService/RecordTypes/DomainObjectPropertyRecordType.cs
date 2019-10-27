using AutoMapper;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Domain Object Property")]
    [EditAction("Domain Object Property")]
    [DeleteAction("Domain Object Property")]
    public class DomainObjectPropertyRecordType : ChangeableRecordType<DomainObjectProperty, long, DomainObjectPropertyBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainObjectProperty, DomainObjectProperty>()
                .ForMember(m => m.Object, o => o.Ignore());
        }
    }
}
