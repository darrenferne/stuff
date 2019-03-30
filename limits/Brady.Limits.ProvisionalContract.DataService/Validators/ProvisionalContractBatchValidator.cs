using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Support.NHibernate.Abstract;

namespace Brady.Limits.ProvisionalContract.DataService.RecordTypes
{
    public class ProvisionalContractBatchValidator : BatchValidator<long, Domain.ProvisionalContract>
    {
        protected override void SetupValidators(ChangeSetItems<long, Domain.ProvisionalContract> items)
        {
            validator = new ProvisionalContractValidator();
            deletionValidator = new ProvisionalContractDeleteValidator();
        }
    }
}