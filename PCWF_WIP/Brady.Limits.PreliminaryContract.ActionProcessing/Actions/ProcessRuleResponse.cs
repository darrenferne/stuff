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
    public class ProcessRuleResponse : AllowedAction<ActionRequest<RuleResponseProcessingPayload>>, IExternalAction
    {
        public ProcessRuleResponse()
            : base()
        { }

        public override IActionResult OnInvoke(ActionRequest<RuleResponseProcessingPayload> request)
        {
            var ruleResponsePayload = request.Payload as RuleResponseProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;

            var newProcessingState = currentProcessingState;

            if (newProcessingState.ContractState.IsPendingApproval.GetValueOrDefault())
            {
                if (ruleResponsePayload.RuleResponse.TriggeredActions.Count == 0)
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
