using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class Cancelled : AllowedState
    {
        public Cancelled()
            : base(nameof(Cancelled))
        { }
    }
}
