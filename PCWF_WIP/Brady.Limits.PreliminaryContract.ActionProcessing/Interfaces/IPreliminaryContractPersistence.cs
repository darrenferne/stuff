using Brady.Limits.PreliminaryContract.Domain.Models;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public interface IPreliminaryContractPersistence 
    {
        ContractTrackingReference GetContractTrackingReference(Guid contractReference);
        (bool IsSaved, string[] Errors) CreateContractTrackingReference(ContractTrackingReference contractTracking);
        (bool IsSaved, string[] Errors) UpdateContractTrackingReference(ContractTrackingReference contractTracking);
    }
}
