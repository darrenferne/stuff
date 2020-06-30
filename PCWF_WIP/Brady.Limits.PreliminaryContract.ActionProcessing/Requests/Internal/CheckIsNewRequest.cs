using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsNewRequest : ActionRequest<ContractProcessingPayload>
    {
        public CheckIsNewRequest(ContractProcessingPayload payload)
            : base(nameof(CheckIsNew), payload)
        { }

        public static CheckIsNewRequest New(ContractProcessingPayload payload) => new CheckIsNewRequest(payload);
    }
}
