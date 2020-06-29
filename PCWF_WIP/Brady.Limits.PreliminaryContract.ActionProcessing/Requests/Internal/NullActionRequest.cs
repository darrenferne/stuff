using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class NullActionRequest : ActionRequest<ContractProcessingPayload>
    {
        public NullActionRequest(ContractProcessingPayload payload)
            : base(nameof(NullAction), payload)
        { }

        public static NullActionRequest New(ContractProcessingPayload payload) => new NullActionRequest(payload);
    }
}
