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
            
            var newProcessingState = currentProcessingState;
            if (!newProcessingState.ContractState.IsMaterialChange.HasValue)
            {
                var isAvailabilityChange = (contractPayload.Contract?.GroupHeader?.HoldFromApproval ?? false) !=
                                     (contractPayload.PreviousVersion?.GroupHeader?.HoldFromApproval ?? false);

                //TODO - Check for material change;
                var isMaterialChange = false;

                //update the external state
                newProcessingState = newProcessingState.Clone(s => s.SetIsMaterialChange(isAvailabilityChange || isMaterialChange)
                                                                    .And()
                                                                    .SetIsAvailable(null));
            }

            //update the public state
            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsMaterialChange());

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
