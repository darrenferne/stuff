using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class ProcessValidContractRequest : GatedActionRequest<Contract>
    {
        public ProcessValidContractRequest(Contract payload)
            : base(nameof(CheckIsOnHold), payload, 
                  new GateDescriptor(nameof(IsOnHold), new ActionRequestDescriptor(nameof(HoldFromApproval))), 
                  new GateDescriptor(nameof(IsNotOnHold), new ActionRequestDescriptor(nameof(Submit))))
        { }
    }
}
