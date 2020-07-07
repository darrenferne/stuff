using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class HoldFromApproval : ExternalState
    {
        public HoldFromApproval()
            : base(nameof(HoldFromApproval),
                  nameof(ValidateContract),
                  nameof(TakeContractOffHold),
                  nameof(NoAction))
        { }
    }
}
