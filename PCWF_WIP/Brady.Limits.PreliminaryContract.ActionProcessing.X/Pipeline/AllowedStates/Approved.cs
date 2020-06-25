using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class Approved : AllowedState
    {
        public Approved()
            : base(nameof(Approved))
        { }
    }
}
