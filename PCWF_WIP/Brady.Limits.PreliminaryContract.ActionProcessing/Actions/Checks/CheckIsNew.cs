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
            var contractProcessingState = request.Context.ProcessingState as ContractProcessingState;

            var newProcessingState = contractProcessingState;
            if (!newProcessingState.ContractState.IsNew.HasValue)
            {
                //TODO - Check if new;
                var isNew = false;
                newProcessingState = newProcessingState.Clone(s => s.SetIsNew(isNew));
            }

            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsNew());

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
