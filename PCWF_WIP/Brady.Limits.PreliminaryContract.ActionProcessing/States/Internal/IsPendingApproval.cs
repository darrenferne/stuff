using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsPendingApproval : InternalState
    {
        public IsPendingApproval()
            : base(nameof(IsPendingApproval),
                  nameof(CheckIsPendingApproval),
                  nameof(ApproveContract),
                  nameof(CancelContract),
                  nameof(RejectContract),
                  nameof(NoAction))
        { }
    }
}
