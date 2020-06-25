using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class PromotionRequest : ActionRequest<Contract>
    {
        public PromotionRequest(Contract payload)
            : base(nameof(Promote), payload)
        { }
    }
}
