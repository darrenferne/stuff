using AutoMapper;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;
using DataServiceDesigner.DataService;

namespace Template.DataService
{
    [CreateAction("Create")]
    [CreateAction("View")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
    public class TemplateRecordType : ChangeableRecordType<DomainObject, long, DomainObjectBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainObject, DomainObject>();
        }
    }
}
