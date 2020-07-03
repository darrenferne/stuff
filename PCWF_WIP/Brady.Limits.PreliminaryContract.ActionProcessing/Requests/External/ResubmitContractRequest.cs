using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ResubmitContractRequest : ActionRequest<ContractProcessingPayload>
    {
        public ResubmitContractRequest(ContractProcessingPayload payload)
            : base(nameof(ResubmitContract), payload)
        { }

        public static ResubmitContractRequest New(ContractProcessingPayload payload) => new ResubmitContractRequest(payload);
    }
}
