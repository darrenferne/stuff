using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsAvailableRequest : ActionRequest<IContractProcessingPayload>
    {
        public CheckIsAvailableRequest(IContractProcessingPayload payload)
            : base(nameof(CheckIsAvailable), payload)
        { }

        public static CheckIsAvailableRequest New(IContractProcessingPayload payload) => new CheckIsAvailableRequest(payload);
    }
}
