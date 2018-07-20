using AutoMapper;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Create")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
    public class DomainObjectPropertyRecordType : ChangeableRecordType<DesignerDomainObjectProperty, long, DomainObjectPropertyBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DesignerDomainObjectProperty, DesignerDomainObjectProperty>()
                .ForMember(x => x.DomainObject, m => m.Ignore());
        }
    }
}
