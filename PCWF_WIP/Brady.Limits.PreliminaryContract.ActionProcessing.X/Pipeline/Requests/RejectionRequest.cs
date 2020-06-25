using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class RejectionRequest : ActionRequest<Contract>
    {
        public RejectionRequest(Contract payload)
            : base(nameof(Reject), payload)
        { }
    }
}
