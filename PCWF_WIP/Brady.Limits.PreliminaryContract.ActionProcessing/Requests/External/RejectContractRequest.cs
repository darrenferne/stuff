using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class RejectContractRequest : ActionRequest<ContractProcessingPayload>
    {
        public RejectContractRequest(ContractProcessingPayload payload)
            : base(nameof(RejectContract), payload)
        { }

        public static RejectContractRequest New(ContractProcessingPayload payload) => new RejectContractRequest(payload);
    }
}
