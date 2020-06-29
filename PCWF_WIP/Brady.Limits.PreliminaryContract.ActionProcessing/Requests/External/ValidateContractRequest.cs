using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ValidateContractRequest : ActionRequest<ContractProcessingPayload>
    {
        public ValidateContractRequest(ContractProcessingPayload payload)
            : base(nameof(ValidateContract), payload)
        { }

        public static ValidateContractRequest New(ContractProcessingPayload payload) => new ValidateContractRequest(payload);
    }
}
