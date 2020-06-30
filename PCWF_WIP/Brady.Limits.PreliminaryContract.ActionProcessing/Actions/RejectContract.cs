using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class RejectContract : AllowedAction<ActionRequest<ContractProcessingPayload>>, IExternalAction
    {
        public RejectContract()
            : base()
        { }

        public override IActionProcessingStateChange OnInvoke(ActionRequest<ContractProcessingPayload> request)
        {
            throw new NotImplementedException();
        }
    }
}
