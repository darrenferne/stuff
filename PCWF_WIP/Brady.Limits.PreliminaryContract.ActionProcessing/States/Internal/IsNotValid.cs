namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsNotValid : IsCheckState
    {
        public IsNotValid()
            : base(nameof(IsNotValid),
                  nameof(ValidateContract),
                  nameof(FailureNotification))
        { }
    }
}
