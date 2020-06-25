namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsNotOnHold : CheckState
    {
        public IsNotOnHold()
            : base(nameof(IsNotOnHold), nameof(Submit))
        { }
    }
}
