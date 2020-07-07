namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsPendingResubmit : IsCheckState
    {
        public IsPendingResubmit()
            : base(nameof(IsPendingResubmit))
        { }
    }
}
