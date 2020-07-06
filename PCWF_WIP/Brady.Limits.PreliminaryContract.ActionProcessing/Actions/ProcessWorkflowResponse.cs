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
            var workflowResponsePayload = request.Payload as WorkflowResponseProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;

            var newProcessingState = currentProcessingState;

            if (newProcessingState.ContractState.IsPendingApproval.GetValueOrDefault())
            {
                if (workflowResponsePayload.WorkflowResponse)
                {
                    //update the external state
                    newProcessingState = newProcessingState.Clone(s => s.SetIsPendingApproval(false)
                                                                        .And()
                                                                        .SetContractStatus(ContractStatus.Approved));
                }
            }

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
