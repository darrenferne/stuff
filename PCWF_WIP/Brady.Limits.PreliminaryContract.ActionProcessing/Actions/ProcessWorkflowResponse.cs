using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Enums;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System.Linq;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ProcessWorkflowResponse : AllowedAction<WorkflowResponseProcessingPayload>, IExternalAction
    {
        IPreliminaryContractPersistence _persistence;
        public ProcessWorkflowResponse(IPreliminaryContractPersistence persistence)
            : base()
        {
            _persistence = persistence;
        }

        public override IActionResult OnInvoke(IActionRequest<WorkflowResponseProcessingPayload> request)
        {
            var workflowResponsePayload = request.Payload as WorkflowResponseProcessingPayload;
            var workflowAction = workflowResponsePayload.WorkflowResponse;

            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;
            var newProcessingState = currentProcessingState;

            if (newProcessingState.ContractState.IsPendingApproval.GetValueOrDefault())
            {
                var trackingReference = workflowResponsePayload.TrackingReference;
                var contractTracking = _persistence.GetContractTrackingReference(trackingReference);
                var actionTracking = contractTracking.Actions.SingleOrDefault(a => a.ActionReference == workflowAction.ActionReference);

                actionTracking.State = workflowAction.ActionState.ToActionTrackingState();

                var newContractStatus = contractTracking.Actions.Any(a => a.State == ActionTrackingState.Rejected) ? ContractStatus.Rejected :
                                        contractTracking.Actions.All(a => a.State == ActionTrackingState.Approved) ? ContractStatus.Approved :
                                        ContractStatus.InFlight;

                if (newContractStatus == ContractStatus.Approved || newContractStatus == ContractStatus.Rejected)
                    contractTracking.State = ContractTrackingState.ActionsComplete;
                else
                    contractTracking.State = ContractTrackingState.ActionsPending;

                var result = _persistence.UpdateContractTrackingReference(contractTracking);
                if (result.IsSaved)
                {
                    if (contractTracking.State != ContractTrackingState.ActionsPending)
                    {
                        //update the external state
                        newProcessingState = newProcessingState.Clone(s => s.SetIsPendingApproval(false)
                                                                            .And()
                                                                            .SetContractStatus(newContractStatus));
                    }
                }

                newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsPendingApproval());
            }

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
