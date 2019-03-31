using AutoMapper;
using BWF.DataServices.Metadata.Attributes.Actions;
using BWF.DataServices.Support.NHibernate.Abstract;

namespace Brady.Limits.DataService.RecordTypes
{
    [CreateAction("Workflow")]
    [EditAction("Workflow")]
    [DeleteAction("Workflow")]
    public class WorkflowRecordType : ChangeableRecordType<Domain.Workflow, long, WorkflowBatchValidator>
    {

        public WorkflowRecordType()
            : base()
        { }

        public override void ConfigureMapper()
        {
            Mapper.CreateMap<Domain.Workflow, Domain.Workflow>()
                .ForMember(m => m.Levels, a => a.Ignore());
        }
    }
}
