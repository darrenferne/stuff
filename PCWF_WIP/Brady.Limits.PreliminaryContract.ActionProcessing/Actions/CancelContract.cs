using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class CancelContract : AllowedAction<ActionRequest<ContractProcessingPayload>>, IExternalAction
    {
        public CancelContract()
            : base()
        { }

        public override IActionResult OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as ContractProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;

            var newProcessingState = currentProcessingState;
            if (newProcessingState.ContractState.IsPendingApproval.GetValueOrDefault())
            {
                //update the external state
                newProcessingState = currentProcessingState.Clone(s => s.SetIsPendingCancel(true));
            }

            //update the public state
            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsPendingCancel());

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
