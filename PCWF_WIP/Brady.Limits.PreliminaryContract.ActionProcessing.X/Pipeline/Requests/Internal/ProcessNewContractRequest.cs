using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class ProcessNewContractRequest : ValidateContractRequest
    {
        public ProcessNewContractRequest(Contract payload)
            : base(payload)
        { }
    }
}
