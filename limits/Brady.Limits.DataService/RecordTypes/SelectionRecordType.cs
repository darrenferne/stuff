using AutoMapper;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;

namespace Brady.Limits.DataService.RecordTypes
{
    [CreateAction("Selection")]
    [EditAction("Selection")]
    [DeleteAction("Selection")]
    public class SelectionRecordType : ChangeableRecordType<Domain.Selection, long, SelectionBatchValidator>
    {

        public SelectionRecordType()
            : base()
        { }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<Domain.Selection, Domain.Selection>();
        }
    }
}
