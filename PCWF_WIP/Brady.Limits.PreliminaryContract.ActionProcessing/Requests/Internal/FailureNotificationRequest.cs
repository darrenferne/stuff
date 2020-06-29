using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class FailureNotificationRequest : ActionRequest<ContractProcessingPayload>
    {
        public FailureNotificationRequest(ContractProcessingPayload payload)
            : base(nameof(FailureNotification), payload)
        { }

        public static FailureNotificationRequest New(ContractProcessingPayload payload) => new FailureNotificationRequest(payload);
    }
}
