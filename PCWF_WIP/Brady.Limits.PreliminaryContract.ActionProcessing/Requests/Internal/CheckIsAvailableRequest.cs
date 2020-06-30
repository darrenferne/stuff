using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsAvailableRequest : ActionRequest<ContractProcessingPayload>
    {
        public CheckIsAvailableRequest(ContractProcessingPayload payload)
            : base(nameof(CheckIsAvailable), payload)
        { }

        public static CheckIsAvailableRequest New(ContractProcessingPayload payload) => new CheckIsAvailableRequest(payload);
    }
}
