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

            var newProcessingState = contractProcessingState;
            if (!newProcessingState.ContractState.IsPendingApproval.HasValue)
            {
                //TODO - Check if inflight;
                var isPendingApproval = false;
                newProcessingState = newProcessingState.Clone(s => s.SetIsPendingApproval(isPendingApproval));
            }

            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsPendingApproval());

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
