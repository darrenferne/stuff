using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    //internal class CheckIsValid : CheckIsValid<ActionRequest<IContractProcessingPayload>>
    //{
    //    public CheckIsValid(IPreliminaryContractValidation validation)
    //        : base(validation)
    //    { }
    //}

    //public abstract class CheckIsValid<TRequest> : AllowedAction<TRequest>
    //    where TRequest : ActionRequest<IContractProcessingPayload>
    public class CheckIsValid : AllowedAction<IContractProcessingPayload>
    {
        IPreliminaryContractValidation _validation;

        public CheckIsValid(IPreliminaryContractValidation validation)
            : base(nameof(CheckIsValid))
        {
            _validation = validation;
        }

        public override IActionResult OnInvoke(IActionRequest<IContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as IContractProcessingPayload;
            var currentProcessingState = request.Context.ProcessingState as ContractProcessingState;
            
            var newProcessingState = currentProcessingState; 
            if (!newProcessingState.ContractState.IsValid.HasValue)
            {
                var result = _validation.ValidateContract(contractPayload.Contract);
                
                //update the external state
                newProcessingState = currentProcessingState.Clone(s => s.SetIsValid(result.IsValid));
            }

            //update the public state
            newProcessingState = newProcessingState.Clone(s => s.SetCurrentFromIsValid());
            
            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
