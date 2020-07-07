using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessUpdatedContractRequest : GatedActionRequest<IContractProcessingPayload>
    {
        public ProcessUpdatedContractRequest(IContractProcessingPayload payload)
            : base(nameof(CheckIsMaterialChange), payload, 
                  new GateDescriptor(nameof(IsMaterialChange), new ActionRequestDescriptor(typeof(ProcessMaterialChangeRequest))),
                  new GateDescriptor(nameof(IsNotMaterialChange), 
                      new GatedRequestDescriptor(nameof(CheckIsPendingApproval), 
                          new GateDescriptor(nameof(IsPendingApproval), new ActionRequestDescriptor(typeof(NoActionRequest))),
                          new GateDescriptor(nameof(IsNotPendingApproval), new ActionRequestDescriptor(typeof(ProcessNewContractRequest))))))
        { }

        public static ProcessUpdatedContractRequest New(IContractProcessingPayload payload) => new ProcessUpdatedContractRequest(payload);
        public static ProcessUpdatedContractRequest New(Contract contract, Guid? trackingReference = null) => new ProcessUpdatedContractRequest(new ContractProcessingPayload(contract, trackingReference));
    }
}
