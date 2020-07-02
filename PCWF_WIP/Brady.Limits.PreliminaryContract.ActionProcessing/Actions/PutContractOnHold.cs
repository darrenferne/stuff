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
    public class PutContractOnHold : AllowedAction<ActionRequest<ContractProcessingPayload>>, IExternalAction
    {
        public PutContractOnHold()
            : base()
        { }

        public override IActionResult OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as ContractProcessingPayload;
            var currentProcessingState = request.Context.CurrentState as ContractProcessingState;
            var currentContractState = currentProcessingState.ContractState;

            var newProcessingState = currentProcessingState;
            if (currentContractState.IsAvailable.GetValueOrDefault())
            {
                var newContractState = currentContractState.Clone().WithIsAvailable(false);
                //update the external state
                newProcessingState = currentProcessingState.Clone(newContractState: newContractState);
            }

            //update the public state
            newProcessingState = newProcessingState.WithCurrentState(nameof(ContractStatus.HoldFromApproval));

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
