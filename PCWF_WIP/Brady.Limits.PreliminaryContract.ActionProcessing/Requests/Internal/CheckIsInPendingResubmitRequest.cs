using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsPendingResubmitRequest : ActionRequest<IContractProcessingPayload>
    {
        public CheckIsPendingResubmitRequest(IContractProcessingPayload payload)
            : base(nameof(CheckIsPendingResubmit), payload)
        { }

        public static CheckIsPendingResubmitRequest New(IContractProcessingPayload payload) => new CheckIsPendingResubmitRequest(payload);
    }
}
