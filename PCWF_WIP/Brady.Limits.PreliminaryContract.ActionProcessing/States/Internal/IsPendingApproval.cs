namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsPendingApproval : IsCheckState
    {
        public IsPendingApproval()
            : base(nameof(IsPendingApproval),
                  nameof(ResubmitContract),
                  nameof(ProcessRuleResponse),
                  nameof(ProcessWorkflowResponse))
        { }
    }
}
