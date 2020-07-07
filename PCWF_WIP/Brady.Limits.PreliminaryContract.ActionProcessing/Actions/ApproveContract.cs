using Brady.Limits.ActionProcessing.Core;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ApproveContract : AllowedAction<IContractProcessingPayload>, IExternalAction
    {
        public ApproveContract()
            : base()
        { }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            throw new NotImplementedException();
        }
    }
}
