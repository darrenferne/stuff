using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class FailureNotification : AllowedAction<FailureNotificationRequest>
    {
        public FailureNotification()
            : base()
        { }

        public override IActionProcessingStateChange OnInvoke(FailureNotificationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
