using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ApprovalRequest : ActionRequest<Contract>
    {
        public ApprovalRequest(Contract payload)
            : base(nameof(Approve), payload)
        { }
    }
}
