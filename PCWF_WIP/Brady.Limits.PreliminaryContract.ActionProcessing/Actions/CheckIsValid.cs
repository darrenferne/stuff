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
    { }

    public abstract class CheckIsValid<TRequest> : AllowedAction<TRequest>
        where TRequest : ActionRequest<ContractProcessingPayload>
    {
        public CheckIsValid()
            : base(nameof(CheckIsValid))
        { }

        public override IActionProcessingStateChange OnInvoke(TRequest request)
        {
            var contract = request.Payload as Contract;
            var contractProcessingState = request.CurrentState as ContractProcessingState;

            var contractState = contractProcessingState.ContractState;
            if (!contractState.IsValid.HasValue)
            {
                //TODO - validate;
                contractState = contractState.Clone().WithIsValid();
            }

            var newProcessingState = contractProcessingState.WithIsValid();

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
