using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsMaterialChange : AllowedAction<CheckIsMaterialChangeRequest>
    {
        public CheckIsMaterialChange()
            : base(nameof(CheckIsMaterialChange))
        { }

        public override IActionProcessingStateChange OnInvoke(CheckIsMaterialChangeRequest request)
        {
            var contract = request.Payload as Contract;
            var contractProcessingState = request.Context.CurrentState as ContractProcessingState;

            var contractState = contractProcessingState.ContractState;
            if (!contractState.IsMaterialChange.HasValue)
            {
                //TODO - Check for material change;
                contractState = contractState.Clone().WithIsMaterialChange();
            }

            var newProcessingState = contractProcessingState.WithIsMaterialChange();

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
