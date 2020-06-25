using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsNew : AllowedAction<ActionRequest<Contract>>
    {
        public CheckIsNew()
            : base()
        { }

        public override IActionProcessingStateChange OnInvoke(ActionRequest<Contract> request)
        {
            var contractProcessingState = request.CurrentState as ContractProcessingState;
            var newProcessingState = contractProcessingState.Clone((contractProcessingState.IsNew ?? false) ? nameof(IsNew) : nameof(IsNotNew), contractProcessingState.ContractState);
            
            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
