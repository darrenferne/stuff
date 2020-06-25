using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class MakeAvailableRequest : ActionRequest<Contract>
    {
        public MakeAvailableRequest(Contract payload)
            : base(nameof(TakeOffHold), payload)
        { }
    }
}
