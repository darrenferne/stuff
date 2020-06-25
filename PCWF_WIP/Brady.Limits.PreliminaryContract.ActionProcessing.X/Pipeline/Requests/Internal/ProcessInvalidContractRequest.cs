using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessInvalidContractRequest : ActionRequest<Contract>
    {
        public ProcessInvalidContractRequest(Contract payload)
            : base(nameof(IsNotValid), payload) //TODO
        { }
    }
}
