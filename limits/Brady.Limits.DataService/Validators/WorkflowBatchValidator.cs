using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;

namespace Brady.Limits.DataService.RecordTypes
{
    public class WorkflowBatchValidator : BatchValidator<long, Domain.Workflow>
    {
        protected override void SetupValidators(ChangeSetItems<long, Domain.Workflow> items)
        {
            validator = new WorkflowValidator();
            deletionValidator = new WorkflowDeleteValidator();
        }
    }
}