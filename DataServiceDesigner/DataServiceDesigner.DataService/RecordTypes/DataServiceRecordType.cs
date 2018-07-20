using AutoMapper;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;
using DataServiceDesigner.Domain;

namespace DataServiceDesigner.DataService
{
    [CreateAction("Create")]
    [EditAction("Edit")]
    [DeleteAction("Delete")]
    public class DataServiceRecordType : ChangeableRecordType<DesignerDataService, long, DataServiceBatchValidator>
    {
        public override void ConfigureMapper()
        {
            Mapper.CreateMap<DesignerDataService, DesignerDataService>()
                .ForMember(x => x.Connection, m => m.Ignore())
                .ForMember(x => x.DomainObjects, m => m.Ignore());
        }
    }
}
