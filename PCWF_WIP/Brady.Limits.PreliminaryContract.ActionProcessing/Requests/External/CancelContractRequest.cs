using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class CancelContractRequest : ActionRequest<ContractProcessingPayload>
    {
        public CancelContractRequest(ContractProcessingPayload payload)
            : base(nameof(CancelContract), payload)
        { }

        public static CancelContractRequest New(ContractProcessingPayload payload) => new CancelContractRequest(payload);
        public static ProcessContractRequest New(Contract contract, Guid? trackingReference = null) => new ProcessContractRequest(new ContractProcessingPayload(contract, trackingReference));
    }
}
