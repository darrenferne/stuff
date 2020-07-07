namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsNotAvailable : IsCheckState
    {
        public IsNotAvailable()
            : base(nameof(IsNotAvailable),
                    nameof(PutContractOnHold))
        { }
    }
}
