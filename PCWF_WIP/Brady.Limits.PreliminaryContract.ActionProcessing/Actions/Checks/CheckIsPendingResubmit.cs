using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsPendingResubmit : AllowedAction<ActionRequest<ContractProcessingPayload>>
    {
        public CheckIsPendingResubmit()
            : base(nameof(CheckIsPendingResubmit))
        { }

        public override IActionResult OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var contractProcessingState = request.Context.ProcessingState as ContractProcessingState;

            var newProcessingState = contractProcessingState.Clone(s => s.SetCurrentFromIsPendingResubmit());

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
