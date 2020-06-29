using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ResubmitContractRequest : ContinuationActionRequest<ContractProcessingPayload>
    {
        public ResubmitContractRequest(ContractProcessingPayload payload)
            : base(nameof(CancelContract), payload, 
                  new ActionRequestDescriptor(typeof(SubmitContractRequest)))
        { }

        public static ResubmitContractRequest New(ContractProcessingPayload payload) => new ResubmitContractRequest(payload);
    }
}
