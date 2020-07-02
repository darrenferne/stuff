using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class HoldFromApproval : ExternalState
    {
        public HoldFromApproval()
            : base(nameof(HoldFromApproval),
                  nameof(ValidateContract),
                  nameof(TakeContractOffHold),
                  nameof(NoAction))
        { }
    }
}
