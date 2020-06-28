using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsNew : AllowedAction<CheckIsNewRequest>
    {
        public CheckIsNew()
            : base(nameof(CheckIsNew))
        { }

        public override IActionProcessingStateChange OnInvoke(CheckIsNewRequest request)
        {
            var contract = request.Payload as Contract;
            var contractProcessingState = request.Context.CurrentState as ContractProcessingState;

            var contractState = contractProcessingState.ContractState;
            if (!contractState.IsNew.HasValue)
            {
                //TODO - Check if new;
                contractState = contractState.Clone().WithIsNew();
            }

            var newProcessingState = contractProcessingState.WithIsNew();

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
