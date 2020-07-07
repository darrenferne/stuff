using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ValidateContract : CheckIsValid, IExternalAction//<ActionRequest<IContractProcessingPayload>>, IExternalAction
    {
        public ValidateContract(IPreliminaryContractValidation validation)
            : base(validation)
        { }
    }
}
