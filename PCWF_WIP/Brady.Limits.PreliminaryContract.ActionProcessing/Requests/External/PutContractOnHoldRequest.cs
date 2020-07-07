using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class PutContractOnHoldRequest : ActionRequest<IContractProcessingPayload>
    {
        public PutContractOnHoldRequest(IContractProcessingPayload payload)
            : base(nameof(PutContractOnHold), payload)
        { }

        public static PutContractOnHoldRequest New(IContractProcessingPayload payload) => new PutContractOnHoldRequest(payload);
    }
}
