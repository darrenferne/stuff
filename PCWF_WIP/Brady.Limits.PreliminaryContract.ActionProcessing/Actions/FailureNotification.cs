using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class FailureNotification : AllowedAction<IContractProcessingPayload>
    {
        public FailureNotification()
            : base()
        { }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;
            
            //TODO
            return new SuccessStateChange(request.Payload, currentProcessingState);
        }
    }
}
