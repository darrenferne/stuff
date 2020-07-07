using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Enums;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class SubmitContract : AllowedAction<IContractProcessingPayload>, IExternalAction
    {
        IPreliminaryContractPersistence _persistence;

        public SubmitContract(IPreliminaryContractPersistence persistence)
            : base()
        {
            _persistence = persistence;
        }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as IContractProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;

            var newProcessingState = currentProcessingState;
            if (!newProcessingState.ContractState.IsPendingApproval.GetValueOrDefault())
            {
                var contractTracking = new ContractTrackingReference(contractPayload.TrackingReference);

                if (_persistence.CreateContractTrackingReference(contractTracking).IsSaved)
                {
                    //update the external state
                    newProcessingState = currentProcessingState.Clone(s => s.SetIsPendingApproval(true));
                }
            }

            if (newProcessingState.ContractState.IsPendingApproval.GetValueOrDefault())
            {
                //update the public state
                newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsPendingApproval()
                                                                    .And()
                                                                    .SetContractStatus(ContractStatus.InFlight));
            }

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
