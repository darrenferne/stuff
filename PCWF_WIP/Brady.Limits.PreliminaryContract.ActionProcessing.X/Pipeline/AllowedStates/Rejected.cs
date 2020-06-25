using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class Rejected : AllowedState
    {
        public Rejected()
            : base(nameof(Rejected))
        { }
    }
}
