using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsPendingApproval : AllowedAction<ActionRequest<ContractProcessingPayload>>
    {
        public CheckIsPendingApproval()
            : base(nameof(CheckIsPendingApproval))
        { }

        public override IActionResult OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as ContractProcessingPayload;
            var contractProcessingState = request.Context.ProcessingState as ContractProcessingState;

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
