using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class InFlight : ExternalState
    {
        public InFlight()
            : base(nameof(InFlight),
                  nameof(NoAction))
        { }
    }
}
