using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class ProcessUpdatedContractRequest : GatedActionRequest<Contract>
    {
        public ProcessUpdatedContractRequest(Contract payload)
            : base(nameof(CheckIsInFlight), payload, 
                new GateDescriptor(nameof(IsInFlight), new ActionRequestDescriptor(typeof(ProcessInFlightContractRequest))),
                new GateDescriptor(nameof(IsNotInFlight), new ActionRequestDescriptor(typeof(ValidateContractRequest))))
        { }
    }
}
