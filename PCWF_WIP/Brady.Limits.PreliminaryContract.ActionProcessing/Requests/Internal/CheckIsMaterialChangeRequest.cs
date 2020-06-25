using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsMaterialChangeRequest : ActionRequest<ContractProcessingPayload>
    {
        public CheckIsMaterialChangeRequest(ContractProcessingPayload payload)
            : base(nameof(CheckIsMaterialChange), payload)
        { }

        public static CheckIsMaterialChangeRequest New(ContractProcessingPayload payload) => new CheckIsMaterialChangeRequest(payload);
    }
}
