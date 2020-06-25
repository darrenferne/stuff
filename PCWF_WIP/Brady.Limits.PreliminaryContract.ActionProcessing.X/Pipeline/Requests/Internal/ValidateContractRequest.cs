using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class ValidateContractRequest : GatedActionRequest<Contract>
    {
        public ValidateContractRequest(Contract payload)
            : base(nameof(CheckIsValid), payload, 
                new GateDescriptor(nameof(IsValid), new ActionRequestDescriptor(typeof(ProcessValidContractRequest))))
        { }
    }
}
