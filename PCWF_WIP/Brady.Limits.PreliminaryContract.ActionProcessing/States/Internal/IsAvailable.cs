namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsAvailable : IsCheckState
    {
        public IsAvailable()
            : base(nameof(IsAvailable),
                  nameof(SubmitContract),
                  nameof(TakeContractOffHold))
        { }
    }
}
