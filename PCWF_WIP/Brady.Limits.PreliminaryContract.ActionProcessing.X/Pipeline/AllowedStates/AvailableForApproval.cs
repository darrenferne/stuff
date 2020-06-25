using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class AvailableForApproval : AllowedState
    {
        public AvailableForApproval()
            : base(nameof(AvailableForApproval))
        { }
    }
}
