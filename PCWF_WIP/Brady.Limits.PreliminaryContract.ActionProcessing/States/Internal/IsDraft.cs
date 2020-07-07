namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsDraft : IsCheckState
    {
        public IsDraft()
            : base(nameof(IsDraft),
                  nameof(ValidateContract),
                  nameof(NoAction))
        { }
    }
}
