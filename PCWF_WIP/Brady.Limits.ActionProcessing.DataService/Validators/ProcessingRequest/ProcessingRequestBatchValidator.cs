using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;
using BWF.DataServices.Support.NHibernate.Validators;

namespace Brady.Limits.ActionProcessing.DataService
{
    public class ProcessingRequestBatchValidator : BatchValidator<long, ProcessingRequest>
    {
        protected override void SetupValidators(ChangeSetItems<long, ProcessingRequest> items)
        {
            if (hasCreatesOrUpdates)
                validator = new ProcessingRequestValidator();

            if (hasDeletes)
                deletionValidator = new EmptyValidator<ProcessingRequest>();
        }
    }
}
