using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessContractRequest : ActionRequest<ContractProcessingPayload>
    {
        public ProcessContractRequest(ContractProcessingPayload payload)
            : base(nameof(ValidateContract), payload)
        { }

        public static ProcessContractRequest New(ContractProcessingPayload payload) => new ProcessContractRequest(payload);
        public static ProcessContractRequest New(Contract contract, Guid? trackingReference = null) => new ProcessContractRequest(new ContractProcessingPayload(contract, trackingReference));
    }
}
