using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsNew : AllowedAction<ActionRequest<ContractProcessingPayload>>
    {
        public CheckIsNew()
            : base(nameof(CheckIsNew))
        { }

        public override IActionResult OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as ContractProcessingPayload;
            var contractProcessingState = request.Context.CurrentState as ContractProcessingState;

            var contractState = contractProcessingState.ContractState;
            if (!contractState.IsNew.HasValue)
            {
                //TODO - Check if new;
                contractState = contractState.Clone().WithIsNew();
            }

            var newProcessingState = contractProcessingState.WithIsNew();

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
