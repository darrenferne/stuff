using Brady.Limits.ActionProcessing.Core;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class Cancel : AllowedAction<CancellationRequest>
    {
        public Cancel()
            : base(nameof(Cancel))
        { }

        public override IActionProcessingStateChange OnInvoke(CancellationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
