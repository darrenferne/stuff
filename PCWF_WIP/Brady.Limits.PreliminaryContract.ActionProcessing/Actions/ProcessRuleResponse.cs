using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Enums;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System.Linq;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessRuleResponse : AllowedAction<RuleResponseProcessingPayload>, IExternalAction
    {
        IPreliminaryContractPersistence _persistence;
        public ProcessRuleResponse(IPreliminaryContractPersistence persistence)
            : base()
        {
            _persistence = persistence;
        }

        public override IActionResult OnInvoke(IActionRequest<RuleResponseProcessingPayload> request)
        {
            var ruleResponsePayload = request.Payload as RuleResponseProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;

            var newProcessingState = currentProcessingState;

            if (newProcessingState.ContractState.IsPendingApproval.GetValueOrDefault())
            {
                var contractTracking = _persistence.GetContractTrackingReference(ruleResponsePayload.TrackingReference);
                if (!(contractTracking is null))
                {
                    contractTracking.State = ruleResponsePayload.RuleResponse.TriggeredActions.Count == 0 ? ContractTrackingState.NoActions : ContractTrackingState.ActionsPending;
                    contractTracking.Actions = ruleResponsePayload.RuleResponse.TriggeredActions.Select(ta => new ActionTrackingReference
                    {
                        State = ActionTrackingState.Pending,
                        ActionReference = ta.ActionReference
                    })
                    .ToList();

                    var result = _persistence.UpdateContractTrackingReference(contractTracking);
                    if (result.IsSaved)
                    {
                        if (ruleResponsePayload.RuleResponse.TriggeredActions.Count == 0)
                        {
                            //update the external state
                            newProcessingState = newProcessingState.Clone(s => s.SetIsPendingApproval(false)
                                                                                .And()
                                                                                .SetContractStatus(ContractStatus.Approved));
                        }
                    }

                    newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsPendingApproval());
                }
            }

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
