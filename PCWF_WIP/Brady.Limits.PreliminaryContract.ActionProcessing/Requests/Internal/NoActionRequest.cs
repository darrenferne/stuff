using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class NoActionRequest : ActionRequest<IContractProcessingPayload>
    {
        public NoActionRequest(IContractProcessingPayload payload)
            : base(nameof(NoAction), payload)
        { }

        public static NoActionRequest New(IContractProcessingPayload payload) => new NoActionRequest(payload);
    }
}
