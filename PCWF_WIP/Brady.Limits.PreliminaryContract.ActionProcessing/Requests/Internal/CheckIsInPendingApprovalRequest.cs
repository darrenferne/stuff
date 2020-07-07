using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsPendingApprovalRequest : ActionRequest<IContractProcessingPayload>
    {
        public CheckIsPendingApprovalRequest(IContractProcessingPayload payload)
            : base(nameof(CheckIsPendingApproval), payload)
        { }

        public static CheckIsPendingApprovalRequest New(IContractProcessingPayload payload) => new CheckIsPendingApprovalRequest(payload);
    }
}
