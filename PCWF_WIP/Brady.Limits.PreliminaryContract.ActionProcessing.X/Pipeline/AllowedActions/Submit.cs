using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class Submit : AllowedAction<ActionRequest<Contract>>
    {
        public Submit()
            : base(nameof(Submit))
        { }

        public override IActionProcessingStateChange OnInvoke(ActionRequest<Contract> request)
        {
            var contractProcessingState = request.CurrentState as ContractProcessingState;
            return new SuccessStateChange(request.Payload, contractProcessingState.Clone(nameof(AvailableForApproval)));
        }
    }
}
