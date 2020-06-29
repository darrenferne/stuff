using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsPendingApprovalRequest : ActionRequest<ContractProcessingPayload>
    {
        public CheckIsPendingApprovalRequest(ContractProcessingPayload payload)
            : base(nameof(CheckIsPendingApproval), payload)
        { }

        public static CheckIsPendingApprovalRequest New(ContractProcessingPayload payload) => new CheckIsPendingApprovalRequest(payload);
    }
}
