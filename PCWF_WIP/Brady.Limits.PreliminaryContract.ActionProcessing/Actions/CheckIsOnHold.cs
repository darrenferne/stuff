using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsOnHold : AllowedAction<CheckIsOnHoldRequest>
    {
        public CheckIsOnHold()
            : base(nameof(CheckIsOnHold))
        { }

        public override IActionProcessingStateChange OnInvoke(CheckIsOnHoldRequest request)
        {
            var contract = request.Payload as Contract;
            var contractProcessingState = request.CurrentState as ContractProcessingState;

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
