using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class CancellationRequest : ActionRequest<Contract>
    {
        public CancellationRequest(Contract payload)
            : base(nameof(Cancel), payload)
        { }
    }
}
