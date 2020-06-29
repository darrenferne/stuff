using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsPendingApproval : AllowedAction<CheckIsPendingApprovalRequest>
    {
        public CheckIsPendingApproval()
            : base(nameof(CheckIsPendingApproval))
        { }

        public override IActionProcessingStateChange OnInvoke(CheckIsPendingApprovalRequest request)
        {
            var contract = request.Payload as Contract;
            var contractProcessingState = request.Context.CurrentState as ContractProcessingState;

            var contractState = contractProcessingState.ContractState;
            if (!contractState.IsPendingApproval.HasValue)
            {
                //TODO - Check if inflight;
                contractState = contractState.Clone().WithIsPendingApproval();
            }

            var newProcessingState = contractProcessingState.WithIsPendingApproval();

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
