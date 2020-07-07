using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ValidateContractRequest : ActionRequest<IContractProcessingPayload>
    {
        public ValidateContractRequest(IContractProcessingPayload payload)
            : base(nameof(ValidateContract), payload)
        { }

        public static ValidateContractRequest New(IContractProcessingPayload payload) => new ValidateContractRequest(payload);
    }
}
