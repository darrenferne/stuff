namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsApproved : IsCheckState
    {
        public IsApproved()
            : base(nameof(IsApproved),
                  nameof(NoAction))
        { }
    }
}
