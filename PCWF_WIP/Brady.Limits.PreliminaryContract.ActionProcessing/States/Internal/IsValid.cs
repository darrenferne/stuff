namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsValid : IsCheckState
    {
        public IsValid()
            : base(nameof(IsValid),
                  nameof(ValidateContract))
        { }
    }
}
