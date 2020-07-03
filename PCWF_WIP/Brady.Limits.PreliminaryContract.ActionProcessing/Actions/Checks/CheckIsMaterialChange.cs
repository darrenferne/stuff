using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsMaterialChange : AllowedAction<ActionRequest<ContractProcessingPayload>>
    {
        public CheckIsMaterialChange()
            : base(nameof(CheckIsMaterialChange))
        { }

        public override IActionResult OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as ContractProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;
            var currentContractState = currentProcessingState.ContractState;

            var newProcessingState = currentProcessingState;
            if (!currentContractState.IsMaterialChange.HasValue)
            {
                var isTakeOffHold = !(contractPayload.Contract?.GroupHeader?.HoldFromApproval ?? false) &&
                                     (contractPayload.PreviousVersion?.GroupHeader?.HoldFromApproval ?? false);

                //TODO - Check for material change;
                var isMaterialChange = false;

                var newContractState = currentContractState.WithIsMaterialChange(isTakeOffHold || isMaterialChange);
                
                //A material change could affect availability so reset the is available flag
                if (newContractState.IsAvailable.HasValue)
                    newContractState = newContractState.WithIsAvailable(null);

                //update the external state
                newProcessingState = currentProcessingState.Clone(newContractState: newContractState);
            }

            //update the public state
            newProcessingState = newProcessingState.WithIsMaterialChange();

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
