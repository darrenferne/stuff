using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class Promoted : AllowedState
    {
        public Promoted()
            : base(nameof(Approve))
        { }
    }
}
