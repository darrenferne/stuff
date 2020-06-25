using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Enums;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsInFlight : AllowedAction<ActionRequest<Contract>>
    {
        public CheckIsInFlight()
            : base()
        { }

        public override IActionProcessingStateChange OnInvoke(ActionRequest<Contract> request)
        {
            var contractProcessingState = request.CurrentState as ContractProcessingState;
            if (!contractProcessingState.IsInFlight.HasValue)
            {
                var contractState = contractProcessingState.ContractState as ContractState;
                contractProcessingState = contractProcessingState.WithIsInFlight(contractState.Name == nameof(ContractStatus.InFlight));
            }
            var newProcessingState = contractProcessingState.Clone((contractProcessingState.IsInFlight ?? false) ? nameof(IsInFlight) : nameof(IsNotInFlight), contractProcessingState.ContractState);

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
