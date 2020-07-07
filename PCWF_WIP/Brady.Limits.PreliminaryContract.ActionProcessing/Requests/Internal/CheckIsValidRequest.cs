using Brady.Limits.ActionProcessing.Core;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsValidRequest : ActionRequest<IContractProcessingPayload>
    {
        public CheckIsValidRequest(IContractProcessingPayload payload)
            : base(nameof(CheckIsValid), payload)
        { }

        public static CheckIsValidRequest New(IContractProcessingPayload payload, Guid trackingReference) => new CheckIsValidRequest(payload);
    }
}
