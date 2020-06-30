using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessNewContractRequest : GatedActionRequest<ContractProcessingPayload>
    {
        public ProcessNewContractRequest(ContractProcessingPayload payload)
            : base(nameof(CheckIsAvailable), payload, 
                  new GateDescriptor(nameof(IsAvailable), new ActionRequestDescriptor(typeof(AutoSubmitContractRequest))),
                  new GateDescriptor(nameof(IsNotAvailable), new ActionRequestDescriptor(typeof(PutContractOnHoldRequest))))
        { }

        public static ProcessNewContractRequest New(ContractProcessingPayload payload) => new ProcessNewContractRequest(payload);
        public static ProcessNewContractRequest New(Contract contract, Guid? trackingReference = null) => new ProcessNewContractRequest(new ContractProcessingPayload(contract, trackingReference));
    }
}
