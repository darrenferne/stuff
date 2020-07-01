using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsNotNew : InternalState
    {
        public IsNotNew()
            : base(nameof(IsNotNew),
                  nameof(CheckIsNew),
                  nameof(CheckIsMaterialChange),
                  nameof(CheckIsPendingApproval),
                  nameof(CheckIsAvailable),
                  nameof(NoAction))
        { }
    }
}
