using Brady.Limits.ActionProcessing.Core;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class RejectContract : AllowedAction<IContractProcessingPayload>, IExternalAction
    {
        public RejectContract()
            : base()
        { }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            throw new NotImplementedException();
        }
    }
}
