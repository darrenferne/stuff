using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class NoAction : AllowedAction<ActionRequest<ContractProcessingPayload>>
    {
        public NoAction()
            : base()
        { }

        public override IActionResult OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            return new SuccessStateChange(request.Payload, request.Context.ProcessingState);
        }
    }
}
