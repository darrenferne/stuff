using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class PendingPromotion : AllowedState
    {
        public PendingPromotion()
            : base(nameof(PendingPromotion))
        { }
    }
}
