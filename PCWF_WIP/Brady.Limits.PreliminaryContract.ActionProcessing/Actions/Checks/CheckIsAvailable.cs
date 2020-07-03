using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsAvailable : AllowedAction<ActionRequest<ContractProcessingPayload>>
    {
        public CheckIsAvailable()
            : base(nameof(CheckIsAvailable))
        { }

        public override IActionResult OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as ContractProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;
            var currentContractState = currentProcessingState.ContractState;

            var newProcessingState = currentProcessingState;
            if (!currentContractState.IsAvailable.HasValue)
            {
                var onHold = contractPayload.Contract.GroupHeader.HoldFromApproval;
                
                //update the external state
                newProcessingState = newProcessingState.Clone(s => s.SetIsAvailable(!onHold));
            }

            //update the public state
            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsAvailable());

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
