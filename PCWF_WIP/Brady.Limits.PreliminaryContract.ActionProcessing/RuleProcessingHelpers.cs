using Brady.Limits.PreliminaryContract.Domain;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public static class RuleProcessingHelpers
    {
        public static ActionTrackingState ToActionTrackingState(this ActionState state)
        {
            switch (state)
            {
                case ActionState.Approved:
                    return ActionTrackingState.Approved;
                case ActionState.Rejected:
                    return ActionTrackingState.Rejected;
                case ActionState.InProgress:
                    return ActionTrackingState.Processing;
                default:
                    return ActionTrackingState.Pending;
            }
        }

    }
}
