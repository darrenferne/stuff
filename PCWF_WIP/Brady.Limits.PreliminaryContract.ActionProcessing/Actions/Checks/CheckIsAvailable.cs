using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsAvailable : AllowedAction<CheckIsAvailableRequest>
    {
        public CheckIsAvailable()
            : base(nameof(CheckIsAvailable))
        { }

        public override IActionProcessingStateChange OnInvoke(CheckIsAvailableRequest request)
        {
            var contract = request.Payload as Contract;
            var contractProcessingState = request.Context.CurrentState as ContractProcessingState;

            var contractState = contractProcessingState.ContractState;
            if (!contractState.IsOnHold.HasValue)
            {
                //TODO - Check if OnHold;
                contractState = contractState.Clone().WithIsOnHold();
            }

            var newProcessingState = contractProcessingState.WithIsOnHold();

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
