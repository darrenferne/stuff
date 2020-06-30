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

        public override IActionProcessingStateChange OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as ContractProcessingPayload;
            var currentProcessingState = request.Context.CurrentState as ContractProcessingState;
            var currentContractState = currentProcessingState.ContractState;

            var newProcessingState = currentProcessingState;
            if (!currentContractState.IsAvailable.HasValue)
            {
                var onHold = contractPayload.Contract.GroupHeader.HoldFromApproval;
                var newContractState = currentContractState.Clone().WithIsAvailable(!onHold);
                //update the external state
                newProcessingState = currentProcessingState.Clone(newContractState: newContractState);
            }

            //update the public state
            newProcessingState = newProcessingState.WithIsAvailable();

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
