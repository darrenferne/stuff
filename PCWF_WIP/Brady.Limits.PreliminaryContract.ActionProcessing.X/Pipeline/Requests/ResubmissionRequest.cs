using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ResubmissionRequest : ContinuationActionRequest<Contract>
    {
        public ResubmissionRequest(Contract payload)
            : base(nameof(Cancel), payload, new ActionRequestDescriptor(typeof(ValidateContractRequest)))
        { }
    }
}
