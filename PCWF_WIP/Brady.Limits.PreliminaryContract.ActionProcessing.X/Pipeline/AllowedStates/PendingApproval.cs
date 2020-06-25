using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class PendingApproval : AllowedState
    {
        public PendingApproval()
            : base(nameof(PendingApproval))
        { }
    }
}
