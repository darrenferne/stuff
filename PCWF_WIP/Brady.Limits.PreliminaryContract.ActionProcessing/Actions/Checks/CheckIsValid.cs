using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsValid : CheckIsValid<ActionRequest<ContractProcessingPayload>>
    {
        public CheckIsValid(IPreliminaryContractValidation validation)
            : base(validation)
        { }
    }

    public abstract class CheckIsValid<TRequest> : AllowedAction<TRequest>
        where TRequest : ActionRequest<ContractProcessingPayload>
    {
        IPreliminaryContractValidation _validation;

        public CheckIsValid(IPreliminaryContractValidation validation)
            : base(nameof(CheckIsValid))
        {
            _validation = validation;
        }

        public override IActionProcessingStateChange OnInvoke(TRequest request)
        {
            var contract = request.Payload as Contract;
            var contractProcessingState = request.Context.CurrentState as ContractProcessingState;

            var contractState = contractProcessingState.ContractState;
            if (!contractState.IsValid.HasValue)
            {
                var result = _validation.ValidateContract(request.Payload.Object as Contract);
                contractState = contractState.Clone().WithIsValid(result.IsValid);
            }

            var newProcessingState = contractProcessingState.WithIsValid();

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
