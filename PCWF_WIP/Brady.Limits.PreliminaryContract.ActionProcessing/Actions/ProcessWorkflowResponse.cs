using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessWorkflowResponse : AllowedAction<ActionRequest<WorkflowResponseProcessingPayload>>, IExternalAction
    {
        public ProcessWorkflowResponse()
            : base()
        { }

        public override IActionResult OnInvoke(ActionRequest<WorkflowResponseProcessingPayload> request)
        {
            throw new NotImplementedException();
        }
    }
}
