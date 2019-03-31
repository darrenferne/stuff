using AutoMapper;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;

namespace Brady.Limits.DataService.RecordTypes
{
    [CreateAction("Workflow Level")]
    [EditAction("Workflow Level")]
    [DeleteAction("Workflow Level")]
    public class WorkflowLevelRecordType : ChangeableRecordType<Domain.WorkflowLevel, long, WorkflowLevelBatchValidator>
    {

        public WorkflowLevelRecordType()
            : base()
        { }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<Domain.WorkflowLevel, Domain.WorkflowLevel>();
        }
    }
}
