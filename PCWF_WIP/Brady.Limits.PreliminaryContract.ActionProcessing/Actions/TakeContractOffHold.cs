using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Enums;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class TakeContractOffHold : AllowedAction<IContractProcessingPayload>, IExternalAction
    {
        public TakeContractOffHold()
            : base()
        { }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as IContractProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;
            var currentContractState = currentProcessingState.ContractState;

            var newProcessingState = currentProcessingState;
            var newContractState = currentContractState;

            if (!currentContractState.IsAvailable.GetValueOrDefault())
            {
                //update the extended state
                newProcessingState = newProcessingState.Clone(s => s.SetIsAvailable(true));
            }

            //set the current state
            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsAvailable()
                                                               .And()
                                                               .SetContractStatus(ContractStatus.AvailableForApproval));

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
