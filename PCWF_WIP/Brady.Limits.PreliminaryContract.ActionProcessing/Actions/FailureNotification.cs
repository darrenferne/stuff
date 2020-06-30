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

        public override IActionProcessingStateChange OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var currentProcessingState = request.Context.CurrentState as ContractProcessingState;
            
            //TODO
            return new SuccessStateChange(request.Payload, currentProcessingState);
        }
    }
}
