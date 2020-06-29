using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessValidContractRequest : GatedActionRequest<ContractProcessingPayload>
    {
        public ProcessValidContractRequest(ContractProcessingPayload payload)
            : base(nameof(CheckIsNew), payload, 
                  new GateDescriptor(nameof(IsNew), new ActionRequestDescriptor(typeof(ProcessNewContractRequest))),
                  new GateDescriptor(nameof(IsNotNew), new ActionRequestDescriptor(typeof(ProcessUpdatedContractRequest))))
        { }

        public static ProcessValidContractRequest New(ContractProcessingPayload payload) => new ProcessValidContractRequest(payload);
        public static ProcessValidContractRequest New(Contract contract, Guid? trackingReference = null) => new ProcessValidContractRequest(new ContractProcessingPayload(contract, trackingReference));
    }
}
