using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsOnHoldRequest : ActionRequest<ContractProcessingPayload>
    {
        public CheckIsOnHoldRequest(ContractProcessingPayload payload)
            : base(nameof(CheckIsOnHold), payload)
        { }

        public static CheckIsOnHoldRequest New(ContractProcessingPayload payload) => new CheckIsOnHoldRequest(payload);
    }
}
