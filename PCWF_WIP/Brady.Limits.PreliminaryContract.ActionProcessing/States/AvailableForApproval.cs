using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class AvailableForApproval : ExternalState
    {
        public AvailableForApproval()
            : base(nameof(AvailableForApproval),
                  nameof(SubmitContract),
                  nameof(PutContractOnHold),
                  nameof(NoAction))
        { }
    }
}
