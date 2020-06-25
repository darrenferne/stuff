using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class SubmissionRequest : ActionRequest<Contract>
    {
        public SubmissionRequest(Contract payload)
            : base(nameof(Submit), payload)
        { }
    }
}
