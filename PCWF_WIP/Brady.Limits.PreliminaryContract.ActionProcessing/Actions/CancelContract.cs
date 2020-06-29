using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class CancelContract : AllowedAction<CancelContractRequest>, IExternalAction
    {
        public CancelContract()
            : base()
        { }

        public override IActionProcessingStateChange OnInvoke(CancelContractRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
