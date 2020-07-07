namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsPendingCancel : IsCheckState
    {
        public IsPendingCancel()
            : base(nameof(IsPendingCancel),
                  nameof(CancelContract))
        { }
    }
}
