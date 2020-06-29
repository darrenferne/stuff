using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class PutContractOnHoldRequest : ActionRequest<ContractProcessingPayload>
    {
        public PutContractOnHoldRequest(ContractProcessingPayload payload)
            : base(nameof(PutContractOnHold), payload)
        { }

        public static PutContractOnHoldRequest New(ContractProcessingPayload payload) => new PutContractOnHoldRequest(payload);
    }
}
