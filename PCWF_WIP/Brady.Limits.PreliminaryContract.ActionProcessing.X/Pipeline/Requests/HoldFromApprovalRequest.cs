using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class HoldFromApprovalRequest : ActionRequest<Contract>
    {
        public HoldFromApprovalRequest(Contract payload)
            : base(nameof(PutOnHold), payload)
        { }
    }
}
