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
    public class TakeContractOffHold : AllowedAction<ActionRequest<ContractProcessingPayload>>, IExternalAction
    {
        public TakeContractOffHold()
            : base()
        { }

        public override IActionResult OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as ContractProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;
            var currentContractState = currentProcessingState.ContractState;

            var newProcessingState = currentProcessingState;
            var newContractState = currentContractState;

            if (!currentContractState.IsAvailable.GetValueOrDefault())
            {
                //update the extended state
                newProcessingState = newProcessingState.Clone(s => s.SetIsAvailable(true));
            }

            //set the current state
            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsAvailable()
                                                               .And()
                                                               .SetContractStatus(ContractStatus.AvailableForApproval));

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
