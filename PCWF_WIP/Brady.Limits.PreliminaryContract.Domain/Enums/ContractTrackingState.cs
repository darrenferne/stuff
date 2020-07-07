namespace Brady.Limits.PreliminaryContract.Domain.Models
{
    public enum ContractTrackingState
    {
        EvaluationPending = 0,
        NoActions = 1,
        ActionsPending = 2,
        ActionsComplete = 3
    }
}