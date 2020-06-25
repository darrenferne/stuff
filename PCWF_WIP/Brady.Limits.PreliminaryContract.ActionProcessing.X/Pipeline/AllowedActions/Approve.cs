using Brady.Limits.ActionProcessing.Core;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class Approve : AllowedAction<ApprovalRequest>
    {
        public Approve()
            : base(nameof(Approve))
        { }

        public override IActionProcessingStateChange OnInvoke(ApprovalRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
