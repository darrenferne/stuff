using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsMaterialChange : AllowedAction<ActionRequest<Contract>>
    {
        public CheckIsMaterialChange()
            : base()
        { }

        public override IActionProcessingStateChange OnInvoke(ActionRequest<Contract> request)
        {

            //TODO
            return new SuccessStateChange(request.Payload, ContractProcessingState.New(nameof(IsNotMaterialChange)));
        }
    }
}
