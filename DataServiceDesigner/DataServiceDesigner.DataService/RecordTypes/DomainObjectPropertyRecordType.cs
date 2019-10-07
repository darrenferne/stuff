using AutoMapper;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Create")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
    public class DomainObjectPropertyRecordType : ChangeableRecordType<DomainObjectProperty, long, DomainObjectPropertyBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainObjectProperty, DomainObjectProperty>()
                .ForMember(x => x.DataService, m => m.Ignore())
                .ForMember(x => x.Object, m => m.Ignore());
        }
    }
}
