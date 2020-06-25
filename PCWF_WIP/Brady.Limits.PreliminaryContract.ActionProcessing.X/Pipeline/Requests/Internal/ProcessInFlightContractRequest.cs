using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class ProcessInFlightContractRequest : GatedActionRequest<Contract>
    {
        public ProcessInFlightContractRequest(Contract payload)
            : base(nameof(CheckIsMaterialChange), payload, 
                  new GateDescriptor(nameof(IsMaterialChange), new ActionRequestDescriptor(nameof(ResubmissionRequest))), 
                  new GateDescriptor(nameof(IsNotMaterialChange), new ActionRequestDescriptor(nameof(ValidateContractRequest))))
        { }
    }
}
