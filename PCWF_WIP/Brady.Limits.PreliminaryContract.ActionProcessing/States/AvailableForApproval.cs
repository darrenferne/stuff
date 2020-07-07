using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class AvailableForApproval : ExternalState
    {
        public AvailableForApproval()
            : base(nameof(AvailableForApproval),
                  nameof(SubmitContract),
                  nameof(PutContractOnHold),
                  nameof(NoAction))
        { }
    }
}
