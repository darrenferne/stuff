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

        public override IActionProcessingStateChange OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as ContractProcessingPayload;
            var contractProcessingState = request.Context.CurrentState as ContractProcessingState;

            var contractState = contractProcessingState.ContractState;
            if (!contractState.IsMaterialChange.HasValue)
            {
                var isTakeOffHold = !(contractPayload.Contract?.GroupHeader?.HoldFromApproval ?? false) &&
                                     (contractPayload.PreviousVersion?.GroupHeader?.HoldFromApproval ?? false);
                
                //TODO - Check for material change;
                var isMaterialChange = false;

                contractState = contractState.Clone().WithIsMaterialChange(isTakeOffHold || isMaterialChange);
            }

            var newProcessingState = contractProcessingState.WithIsMaterialChange();

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
