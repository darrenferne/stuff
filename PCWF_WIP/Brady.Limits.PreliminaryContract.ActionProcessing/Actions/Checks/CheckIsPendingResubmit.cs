using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsPendingResubmit : AllowedAction<IContractProcessingPayload>
    {
        public CheckIsPendingResubmit()
            : base(nameof(CheckIsPendingResubmit))
        { }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            var contractProcessingState = request.Context.ProcessingState as ContractProcessingState;

            var newProcessingState = contractProcessingState.Clone(s => s.SetCurrentFromIsPendingResubmit());

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
