using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public interface IPreliminaryContractValidation
    {
        (bool IsValid, string[] Errors) ValidateContract(Contract contract);
    }
}
