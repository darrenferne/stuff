using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;

namespace Brady.Limits.DataService.RecordTypes
{
    public class SelectionBatchValidator : BatchValidator<long, Domain.Selection>
    {
        protected override void SetupValidators(ChangeSetItems<long, Domain.Selection> items)
        {
            validator = new SelectionValidator();
            deletionValidator = new SelectionDeleteValidator();
        }
    }
}