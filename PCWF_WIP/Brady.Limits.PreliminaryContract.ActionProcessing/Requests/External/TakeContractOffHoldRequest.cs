using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class TakeContractOffHoldRequest : ActionRequest<IContractProcessingPayload>
    {
        public TakeContractOffHoldRequest(IContractProcessingPayload payload)
            : base(nameof(TakeContractOffHold), payload)
        { }

        public static TakeContractOffHoldRequest New(IContractProcessingPayload payload) => new TakeContractOffHoldRequest(payload);
    }
}
