namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsNotPendingResubmit : IsCheckState
    {
        public IsNotPendingResubmit()
            : base(nameof(IsNotPendingResubmit))
        { }
    }
}
