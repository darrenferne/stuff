using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Enums;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ResubmitContract : CancelContract
    {
        public ResubmitContract()
            : base()
        { }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            var cancelResult = base.OnInvoke(request);

            if (!(cancelResult is SuccessStateChange))
                return cancelResult;

            var newProcessingState = (cancelResult as SuccessStateChange).NewState as ContractProcessingState;
            if (!newProcessingState.ContractState.IsPendingResubmit.GetValueOrDefault())
            {
                //update the external state
                newProcessingState = newProcessingState.Clone(s => s.SetIsPendingResubmit(true));
            }

            //update the public state
            newProcessingState = newProcessingState.Clone(s =>  s.SetCurrentFromIsPendingResubmit()
                                                                .And()
                                                                .SetContractStatus(ContractStatus.InFlight));

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
