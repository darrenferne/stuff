using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class NoAction : AllowedAction<IContractProcessingPayload>
    {
        public NoAction()
            : base()
        { }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            return new SuccessStateChange(request.Payload, request.Context.ProcessingState);
        }
    }
}
