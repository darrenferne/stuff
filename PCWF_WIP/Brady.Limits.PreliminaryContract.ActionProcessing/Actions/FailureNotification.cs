using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class FailureNotification : AllowedAction<ActionRequest<ContractProcessingPayload>>
    {
        public FailureNotification()
            : base()
        { }

        public override IActionResult OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;
            
            //TODO
            return new SuccessStateChange(request.Payload, currentProcessingState);
        }
    }
}
