using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessContractRequest : GatedActionRequest<ContractProcessingPayload>
    {
        public ProcessContractRequest(ContractProcessingPayload payload)
            : base(nameof(ValidateContract), payload,
                  new GateDescriptor(nameof(IsValid), new ActionRequestDescriptor(typeof(ProcessValidContractRequest))),
                  new GateDescriptor(nameof(IsNotValid), new ActionRequestDescriptor(typeof(FailureNotificationRequest))))
        { }

        public static ProcessContractRequest New(ContractProcessingPayload payload) => new ProcessContractRequest(payload);
        public static ProcessContractRequest New(Contract contract, Guid? trackingReference = null) => new ProcessContractRequest(new ContractProcessingPayload(contract, trackingReference));
        public static ProcessContractRequest New(Contract currentContract, Contract previousVersion, Guid trackingReference) => new ProcessContractRequest(new ContractProcessingPayload(currentContract, previousVersion, trackingReference));
    }
}
