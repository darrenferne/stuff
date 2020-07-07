using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class MakeContractAvailableRequest : ActionRequest<IContractProcessingPayload>
    {
        public MakeContractAvailableRequest(IContractProcessingPayload payload)
            : base(nameof(TakeContractOffHold), payload)
        { }

        public static MakeContractAvailableRequest New(IContractProcessingPayload payload) => new MakeContractAvailableRequest(payload);
    }
}
