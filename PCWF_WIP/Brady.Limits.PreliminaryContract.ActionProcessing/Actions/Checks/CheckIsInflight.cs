using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsInflight : AllowedAction<ActionRequest<ContractProcessingPayload>>
    {
        public CheckIsInflight()
            : base(nameof(CheckIsInflight))
        { }

        public override IActionResult OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as ContractProcessingPayload;
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
