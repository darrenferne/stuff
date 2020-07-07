using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsMaterialChange : AllowedAction<IContractProcessingPayload>
    {
        public CheckIsMaterialChange()
            : base(nameof(CheckIsMaterialChange))
        { }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as IContractProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;
            
            var newProcessingState = currentProcessingState;
            if (!newProcessingState.ContractState.IsMaterialChange.HasValue)
            {
                var isAvailabilityChange = (contractPayload.Contract?.GroupHeader?.HoldFromApproval ?? false) !=
                                     (contractPayload.PreviousVersion?.GroupHeader?.HoldFromApproval ?? false);

                //TODO - Check for material change;
                var isMaterialChange = false;

                //update the external state
                newProcessingState = newProcessingState.Clone(s => s.SetIsMaterialChange(isAvailabilityChange || isMaterialChange)
                                                                    .And()
                                                                    .SetIsAvailable(null));
            }

            //update the public state
            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsMaterialChange());

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
