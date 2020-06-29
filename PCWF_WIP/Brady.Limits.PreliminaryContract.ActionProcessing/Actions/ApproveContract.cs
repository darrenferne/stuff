using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ApproveContract : AllowedAction<ApproveContractRequest>, IExternalAction
    {
        public ApproveContract()
            : base()
        { }

        public override IActionProcessingStateChange OnInvoke(ApproveContractRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
