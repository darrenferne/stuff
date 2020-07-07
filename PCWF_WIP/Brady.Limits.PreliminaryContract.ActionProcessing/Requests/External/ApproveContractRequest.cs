using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ApproveContractRequest : ActionRequest<IContractProcessingPayload>
    {
        public ApproveContractRequest(IContractProcessingPayload payload)
            : base(nameof(SubmitContract), payload)
        { }

        public static ApproveContractRequest New(IContractProcessingPayload payload) => new ApproveContractRequest(payload);
    }
}
