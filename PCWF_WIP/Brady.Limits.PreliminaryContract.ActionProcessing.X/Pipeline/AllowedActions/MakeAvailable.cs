using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class MakeAvailable : AllowedAction<MakeAvailableRequest>
    {
        public MakeAvailable()
            : base(nameof(MakeAvailable))
        { }

        public override IActionProcessingStateChange OnInvoke(MakeAvailableRequest request)
        {
            var contractProcessingState = request.CurrentState as ContractProcessingState;
            return new SuccessStateChange(request.Payload, contractProcessingState.Clone(nameof(AvailableForApproval)));
        }
    }
}
