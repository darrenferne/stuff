using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class PutOnHold : AllowedAction<HoldFromApprovalRequest>
    {
        public PutOnHold()
            : base(nameof(PutOnHold))
        { }

        public override IActionProcessingStateChange OnInvoke(HoldFromApprovalRequest request)
        {
            var contractProcessingState = request.CurrentState as ContractProcessingState;
            return new SuccessStateChange(request.Payload, contractProcessingState.Clone(nameof(HoldFromApproval)));
        }
    }
}
