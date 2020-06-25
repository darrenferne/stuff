using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsValidRequest : ActionRequest<ContractProcessingPayload>
    {
        public CheckIsValidRequest(ContractProcessingPayload payload)
            : base(nameof(CheckIsValid), payload)
        { }

        public static CheckIsValidRequest New(ContractProcessingPayload payload, Guid trackingReference) => new CheckIsValidRequest(payload);
    }
}
