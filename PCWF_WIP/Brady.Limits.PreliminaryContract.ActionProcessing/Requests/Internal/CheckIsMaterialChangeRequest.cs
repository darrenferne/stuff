using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class CheckIsMaterialChangeRequest : ActionRequest<IContractProcessingPayload>
    {
        public CheckIsMaterialChangeRequest(IContractProcessingPayload payload)
            : base(nameof(CheckIsMaterialChange), payload)
        { }

        public static CheckIsMaterialChangeRequest New(IContractProcessingPayload payload) => new CheckIsMaterialChangeRequest(payload);
    }
}
