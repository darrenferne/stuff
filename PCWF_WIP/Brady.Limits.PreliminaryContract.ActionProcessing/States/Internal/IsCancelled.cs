namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsCancelled : IsCheckState
    {
        public IsCancelled()
            : base(nameof(IsCancelled),
                  nameof(SubmitContract))
        { }
    }
}
