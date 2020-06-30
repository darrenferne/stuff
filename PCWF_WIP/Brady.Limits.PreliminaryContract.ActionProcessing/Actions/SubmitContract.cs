using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Enums;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class SubmitContract : AllowedAction<ActionRequest<ContractProcessingPayload>>, IExternalAction
    {
        public SubmitContract()
            : base()
        { }

        public override IActionProcessingStateChange OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as ContractProcessingPayload;
            var currentProcessingState = request.Context.CurrentState as ContractProcessingState;
            var currentContractState = currentProcessingState.ContractState;

            var newProcessingState = currentProcessingState;
            if (!currentContractState.IsPendingApproval.GetValueOrDefault())
            {
                var newContractState = currentContractState.Clone().WithIsPendingApproval(true);
                //update the external state
                newProcessingState = currentProcessingState.Clone(newContractState: newContractState);
            }

            //update the public state
            newProcessingState = newProcessingState.WithCurrentState(nameof(ContractStatus.InFlight));

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
