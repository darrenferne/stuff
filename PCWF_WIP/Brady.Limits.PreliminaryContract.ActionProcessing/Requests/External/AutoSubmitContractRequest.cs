using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class AutoSubmitContractRequest : ContinuationActionRequest<IContractProcessingPayload>
    {
        public AutoSubmitContractRequest(IContractProcessingPayload payload)
            : base(nameof(TakeContractOffHold), payload, 
                  new ActionRequestDescriptor(typeof(SubmitContractRequest)))
        { }

        public static AutoSubmitContractRequest New(IContractProcessingPayload payload) => new AutoSubmitContractRequest(payload);
    }
}
