using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Enums;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ResubmitContract : CancelContract
    {
        public ResubmitContract()
            : base()
        { }

        public override IActionResult OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            var cancelResult = base.OnInvoke(request);

            if (!(cancelResult is SuccessStateChange))
                return cancelResult;

            var newProcessingState = (cancelResult as SuccessStateChange).NewState as ContractProcessingState;
            if (!newProcessingState.ContractState.IsPendingResubmit.GetValueOrDefault())
            {
                //update the external state
                newProcessingState = newProcessingState.Clone(s => s.SetIsPendingResubmit(true));
            }

            //update the public state
            newProcessingState = newProcessingState.Clone(s =>  s.SetCurrentFromIsPendingResubmit()
                                                                .And()
                                                                .SetContractStatus(ContractStatus.InFlight));

            return new SuccessStateChange(request.Payload, newProcessingState);
        }
    }
}
