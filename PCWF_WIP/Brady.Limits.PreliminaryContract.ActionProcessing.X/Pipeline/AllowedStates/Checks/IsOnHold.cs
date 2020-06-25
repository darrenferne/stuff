namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsOnHold : CheckState
    {
        public IsOnHold()
            : base(nameof(IsOnHold), nameof(PutOnHold))
        { }
    }
}
