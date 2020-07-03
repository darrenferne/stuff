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
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;
            
            var newProcessingState = currentProcessingState;
            if (newProcessingState.ContractState.IsAvailable.GetValueOrDefault())
            {
                //update the external state
                newProcessingState = newProcessingState.Clone(s => s.SetIsAvailable(false));
            }

            //update the public state
            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsAvailable()
                                                                .And()
                                                                .SetContractStatus(ContractStatus.HoldFromApproval));

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
