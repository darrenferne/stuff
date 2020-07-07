using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsPendingApproval : AllowedAction<IContractProcessingPayload>
    {
        public CheckIsPendingApproval()
            : base(nameof(CheckIsPendingApproval))
        { }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as IContractProcessingPayload;
            var contractProcessingState = request.Context.ProcessingState as ContractProcessingState;

            var newProcessingState = contractProcessingState;
            if (!newProcessingState.ContractState.IsPendingApproval.HasValue)
            {
                //TODO - Check if inflight;
                var isPendingApproval = false;
                newProcessingState = newProcessingState.Clone(s => s.SetIsPendingApproval(isPendingApproval));
            }

            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsPendingApproval());

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
