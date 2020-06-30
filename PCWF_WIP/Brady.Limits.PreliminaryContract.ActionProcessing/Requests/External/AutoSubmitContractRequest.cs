using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class AutoSubmitContractRequest : ContinuationActionRequest<ContractProcessingPayload>
    {
        public AutoSubmitContractRequest(ContractProcessingPayload payload)
            : base(nameof(TakeContractOffHold), payload, 
                  new ActionRequestDescriptor(typeof(SubmitContractRequest)))
        { }

        public static AutoSubmitContractRequest New(ContractProcessingPayload payload) => new AutoSubmitContractRequest(payload);
    }
}
