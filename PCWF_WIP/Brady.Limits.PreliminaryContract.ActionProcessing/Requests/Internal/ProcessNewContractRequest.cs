using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessNewContractRequest : GatedActionRequest<IContractProcessingPayload>
    {
        public ProcessNewContractRequest(IContractProcessingPayload payload)
            : base(nameof(CheckIsAvailable), payload, 
                  new GateDescriptor(nameof(IsAvailable), new ActionRequestDescriptor(typeof(AutoSubmitContractRequest))),
                  new GateDescriptor(nameof(IsNotAvailable), new ActionRequestDescriptor(typeof(PutContractOnHoldRequest))))
        { }

        public static ProcessNewContractRequest New(IContractProcessingPayload payload) => new ProcessNewContractRequest(payload);
        public static ProcessNewContractRequest New(Contract contract, Guid? trackingReference = null) => new ProcessNewContractRequest(new ContractProcessingPayload(contract, trackingReference));
    }
}
