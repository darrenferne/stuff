using AutoMapper;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Create")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
    public class DomainSchemaRecordType : ObservableRecordType<DomainSchema, long, DomainSchemaBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DomainSchema, DomainSchema>()
                .ForMember(m => m.DataService, o => o.Ignore())
                .ForMember(m => m.Objects, o => o.Ignore()); 
        }
    }
}
