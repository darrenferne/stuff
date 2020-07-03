using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    //internal class CheckIsValid : CheckIsValid<ActionRequest<ContractProcessingPayload>>
    //{
    //    public CheckIsValid(IPreliminaryContractValidation validation)
    //        : base(validation)
    //    { }
    //}

    //public abstract class CheckIsValid<TRequest> : AllowedAction<TRequest>
    //    where TRequest : ActionRequest<ContractProcessingPayload>
    public class CheckIsValid : AllowedAction<ActionRequest<ContractProcessingPayload>>
    {
        IPreliminaryContractValidation _validation;

        public CheckIsValid(IPreliminaryContractValidation validation)
            : base(nameof(CheckIsValid))
        {
            _validation = validation;
        }

        public override IActionResult OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var contractPayload = request.Payload as ContractProcessingPayload;
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
