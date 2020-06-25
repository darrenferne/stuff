using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ExpirationRequest : ActionRequest<Contract>
    {
        public ExpirationRequest(Contract payload)
            : base(nameof(Expire), payload)
        { }
    }
}
