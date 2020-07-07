namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsNotPendingApproval : IsCheckState
    {
        public IsNotPendingApproval()
            : base(nameof(IsNotPendingApproval),
                  nameof(ResubmitContract))
        { }
    }
}
