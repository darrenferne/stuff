namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsNotPendingCancel : IsCheckState
    {
        public IsNotPendingCancel()
            : base(nameof(IsNotPendingCancel))
        { }
    }
}
