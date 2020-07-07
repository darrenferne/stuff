using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class CancelContractRequest : ActionRequest<IContractProcessingPayload>
    {
        public CancelContractRequest(IContractProcessingPayload payload)
            : base(nameof(CancelContract), payload)
        { }

        public static CancelContractRequest New(IContractProcessingPayload payload) => new CancelContractRequest(payload);
        public static ProcessContractRequest New(Contract contract, Guid? trackingReference = null) => new ProcessContractRequest(new ContractProcessingPayload(contract, trackingReference));
    }
}
