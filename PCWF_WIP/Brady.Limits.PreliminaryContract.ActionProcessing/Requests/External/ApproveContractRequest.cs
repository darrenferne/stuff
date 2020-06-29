using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ApproveContractRequest : ActionRequest<ContractProcessingPayload>
    {
        public ApproveContractRequest(ContractProcessingPayload payload)
            : base(nameof(SubmitContract), payload)
        { }

        public static ApproveContractRequest New(ContractProcessingPayload payload) => new ApproveContractRequest(payload);
    }
}
