using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class SubmitContractRequest : ActionRequest<IContractProcessingPayload>
    {
        public SubmitContractRequest(IContractProcessingPayload payload)
            : base(nameof(SubmitContract), payload)
        { }

        public static SubmitContractRequest New(IContractProcessingPayload payload) => new SubmitContractRequest(payload);
    }
}
