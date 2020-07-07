using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Enums;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class PutContractOnHold : AllowedAction<IContractProcessingPayload>, IExternalAction
    {
        public PutContractOnHold()
            : base()
        { }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as IContractProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;
            
            var newProcessingState = currentProcessingState;
            if (newProcessingState.ContractState.IsAvailable.GetValueOrDefault())
            {
                //update the external state
                newProcessingState = newProcessingState.Clone(s => s.SetIsAvailable(false));
            }

            //update the public state
            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsAvailable()
                                                                .And()
                                                                .SetContractStatus(ContractStatus.HoldFromApproval));

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
