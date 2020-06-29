using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class IsMaterialChange : InternalState
    {
        public IsMaterialChange()
            : base(nameof(IsMaterialChange),
                  nameof(CheckIsMaterialChange),
                  nameof(CheckIsPendingApproval))
        { }
    }
}
