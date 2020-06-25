using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class InitialiseProcessStateRequest : ActionRequest<Contract>
    {
        public InitialiseProcessStateRequest(Contract payload)
            : base(nameof(InitialiseProcessState), payload)
        { }
    }
}
