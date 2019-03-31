using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;

namespace Brady.Limits.DataService.RecordTypes
{
    public class WorkflowLevelBatchValidator : BatchValidator<long, Domain.WorkflowLevel>
    {
        protected override void SetupValidators(ChangeSetItems<long, Domain.WorkflowLevel> items)
        {
            validator = new WorkflowLevelValidator();
            deletionValidator = new WorkflowLevelDeleteValidator();
        }
    }
}