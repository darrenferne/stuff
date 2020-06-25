using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsOnHold : AllowedAction<ActionRequest<Contract>>
    {
        //public CheckIsOnHold()
        //    : base()
        //{ }

        //public override IActionProcessingStateChange OnInvoke(ActionRequest<Contract> request)
        //{
        //    var contractProcessingState = request.CurrentState as ContractProcessingState;
        //    if (!contractProcessingState.IsOnHold.HasValue)
        //    {
        //        var contractHeader = request.Payload.GroupHeader;
        //        var contractState = contractProcessingState.ContractState as ContractState;

        //        var isOnHold = (contractHeader?.HoldFromApproval ?? false) || contractState?.Name == nameof(ContractStatus.HoldFromApproval);
        //        contractProcessingState = contractProcessingState.WithIsOnHold(isOnHold);
        //    }
        //    var newProcessingState = contractProcessingState.Clone((contractProcessingState.IsOnHold ?? false) ? nameof(IsOnHold) : nameof(IsNotOnHold), contractProcessingState.ContractState);

        //    return new SuccessStateChange(request.Payload, newProcessingState);
        //}

        public CheckIsOnHold()
        {

        }

        public override IActionProcessingStateChange OnInvoke(ActionRequest<Contract> request)
        {
            throw new System.NotImplementedException();
        }
    }
}
