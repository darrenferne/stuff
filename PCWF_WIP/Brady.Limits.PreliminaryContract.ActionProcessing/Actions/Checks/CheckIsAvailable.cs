using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsAvailable : AllowedAction<IContractProcessingPayload>
    {
        public CheckIsAvailable()
            : base(nameof(CheckIsAvailable))
        { }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as IContractProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;
            var currentContractState = currentProcessingState.ContractState;

            var newProcessingState = currentProcessingState;
            if (!currentContractState.IsAvailable.HasValue)
            {
                var onHold = contractPayload.Contract.GroupHeader.HoldFromApproval;
                
                //update the external state
                newProcessingState = newProcessingState.Clone(s => s.SetIsAvailable(!onHold));
            }

            //update the public state
            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsAvailable());

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
