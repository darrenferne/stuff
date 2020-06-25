using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class HoldFromApproval : AllowedState
    {
        public HoldFromApproval()
            : base(nameof(HoldFromApproval))
        { }
    }
}
