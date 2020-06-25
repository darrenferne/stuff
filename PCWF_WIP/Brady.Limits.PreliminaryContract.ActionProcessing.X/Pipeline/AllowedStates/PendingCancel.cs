using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class PendingCancel : AllowedState
    {
        public PendingCancel()
            : base(nameof(PendingCancel))
        { }
    }
}
