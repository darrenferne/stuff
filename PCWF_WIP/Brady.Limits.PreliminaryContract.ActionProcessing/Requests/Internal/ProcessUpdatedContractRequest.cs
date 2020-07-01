using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessUpdatedContractRequest : GatedActionRequest<ContractProcessingPayload>
    {
        public ProcessUpdatedContractRequest(ContractProcessingPayload payload)
            : base(nameof(CheckIsMaterialChange), payload, 
                  new GateDescriptor(nameof(IsMaterialChange), new ActionRequestDescriptor(typeof(ProcessMaterialChangeRequest))),
                  new GateDescriptor(nameof(IsNotMaterialChange), 
                      new GatedRequestDescriptor(nameof(CheckIsPendingApproval), 
                          new GateDescriptor(nameof(IsPendingApproval), new ActionRequestDescriptor(typeof(NoActionRequest))),
                          new GateDescriptor(nameof(IsNotPendingApproval), new ActionRequestDescriptor(typeof(ProcessNewContractRequest))))))
        { }

        public static ProcessUpdatedContractRequest New(ContractProcessingPayload payload) => new ProcessUpdatedContractRequest(payload);
        public static ProcessUpdatedContractRequest New(Contract contract, Guid? trackingReference = null) => new ProcessUpdatedContractRequest(new ContractProcessingPayload(contract, trackingReference));
    }
}
