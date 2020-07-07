using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class FailureNotificationRequest : ActionRequest<IContractProcessingPayload>
    {
        public FailureNotificationRequest(IContractProcessingPayload payload)
            : base(nameof(FailureNotification), payload)
        { }

        public static FailureNotificationRequest New(IContractProcessingPayload payload) => new FailureNotificationRequest(payload);
    }
}
