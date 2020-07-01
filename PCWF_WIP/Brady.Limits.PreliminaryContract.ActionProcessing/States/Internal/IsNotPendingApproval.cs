using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsNotPendingApproval : InternalState
    {
        public IsNotPendingApproval()
            : base(nameof(IsNotPendingApproval),
                  nameof(CheckIsPendingApproval),
                  nameof(CheckIsAvailable),
                  nameof(NoAction))
        { }
    }
}
