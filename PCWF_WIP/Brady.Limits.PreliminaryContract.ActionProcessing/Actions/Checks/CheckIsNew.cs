using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsNew : AllowedAction<IContractProcessingPayload>
    {
        public CheckIsNew()
            : base(nameof(CheckIsNew))
        { }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as IContractProcessingPayload;
            var contractProcessingState = request.Context.ProcessingState as ContractProcessingState;

            var newProcessingState = contractProcessingState;
            if (!newProcessingState.ContractState.IsNew.HasValue)
            {
                //TODO - Check if new;
                var isNew = false;
                newProcessingState = newProcessingState.Clone(s => s.SetIsNew(isNew));
            }

            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsNew());

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
