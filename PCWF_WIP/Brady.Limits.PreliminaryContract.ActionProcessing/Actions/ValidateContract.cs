using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class ValidateContract : CheckIsValid, IExternalAction//<ActionRequest<ContractProcessingPayload>>, IExternalAction
    {
        public ValidateContract(IPreliminaryContractValidation validation)
            : base(validation)
        { }
    }
}
