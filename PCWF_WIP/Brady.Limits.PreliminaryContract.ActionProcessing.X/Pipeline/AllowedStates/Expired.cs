using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class Expired : AllowedState
    {
        public Expired()
            : base(nameof(Expired))
        { }
    }
}
