using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ResubmitContractRequest : ActionRequest<IContractProcessingPayload>
    {
        public ResubmitContractRequest(IContractProcessingPayload payload)
            : base(nameof(ResubmitContract), payload)
        { }

        public static ResubmitContractRequest New(IContractProcessingPayload payload) => new ResubmitContractRequest(payload);
    }
}
