using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class RejectContractRequest : ActionRequest<IContractProcessingPayload>
    {
        public RejectContractRequest(IContractProcessingPayload payload)
            : base(nameof(RejectContract), payload)
        { }

        public static RejectContractRequest New(IContractProcessingPayload payload) => new RejectContractRequest(payload);
    }
}
