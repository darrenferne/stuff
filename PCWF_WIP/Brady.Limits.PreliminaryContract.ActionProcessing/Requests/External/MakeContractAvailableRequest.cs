using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class MakeContractAvailableRequest : ActionRequest<ContractProcessingPayload>
    {
        public MakeContractAvailableRequest(ContractProcessingPayload payload)
            : base(nameof(TakeContractOffHold), payload)
        { }

        public static MakeContractAvailableRequest New(ContractProcessingPayload payload) => new MakeContractAvailableRequest(payload);
    }
}
