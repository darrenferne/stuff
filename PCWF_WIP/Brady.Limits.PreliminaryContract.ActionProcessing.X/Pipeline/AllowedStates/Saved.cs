using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class Saved : AllowedState
    {
        public Saved()
            : base(nameof(Saved)
                  , nameof(InitialiseProcessState))
        { }
    }
}
