using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsNewRequest : ActionRequest<IContractProcessingPayload>
    {
        public CheckIsNewRequest(IContractProcessingPayload payload)
            : base(nameof(CheckIsNew), payload)
        { }

        public static CheckIsNewRequest New(IContractProcessingPayload payload) => new CheckIsNewRequest(payload);
    }
}
