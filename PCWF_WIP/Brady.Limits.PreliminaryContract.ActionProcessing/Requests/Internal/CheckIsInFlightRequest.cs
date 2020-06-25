using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsInflightRequest : ActionRequest<ContractProcessingPayload>
    {
        public CheckIsInflightRequest(ContractProcessingPayload payload)
            : base(nameof(CheckIsInflight), payload)
        { }

        public static CheckIsInflightRequest New(ContractProcessingPayload payload) => new CheckIsInflightRequest(payload);
    }
}
