using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class NoActionRequest : ActionRequest<ContractProcessingPayload>
    {
        public NoActionRequest(ContractProcessingPayload payload)
            : base(nameof(NoAction), payload)
        { }

        public static NoActionRequest New(ContractProcessingPayload payload) => new NoActionRequest(payload);
    }
}
