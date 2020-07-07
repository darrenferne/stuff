using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class CancelContract : AllowedAction<IContractProcessingPayload>, IExternalAction
    {
        public CancelContract()
            : base()
        { }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as IContractProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;

            var newProcessingState = currentProcessingState;
            if (newProcessingState.ContractState.IsPendingApproval.GetValueOrDefault())
            {
                //update the external state
                newProcessingState = currentProcessingState.Clone(s => s.SetIsPendingCancel(true));
            }

            //update the public state
            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsPendingCancel());

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
