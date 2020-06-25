using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Validators;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ProcessingStateBatchValidator : BatchValidator<long, ProcessingState>
    {
        protected override void SetupValidators(ChangeSetItems<long, ProcessingState> items)
        {
            if (hasCreatesOrUpdates)
                validator = new ProcessingStateValidator();

            if (hasDeletes)
                deletionValidator = new EmptyValidator<ProcessingState>();
        }
    }
}
