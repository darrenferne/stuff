using Brady.Limits.ActionProcessing.Core;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class Reject : AllowedAction<RejectionRequest>
    {
        public Reject()
            : base(nameof(Reject))
        { }

        public override IActionProcessingStateChange OnInvoke(RejectionRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
