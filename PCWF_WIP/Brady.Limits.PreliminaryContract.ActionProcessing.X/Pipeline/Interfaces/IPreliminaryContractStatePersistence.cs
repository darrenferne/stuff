using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public interface IPreliminaryContractStatePersistence
    {
        void SaveContract(Contract contract);

        ContractState GetState(long contractId);

        ContractState SaveState(ContractState state);
    }
}
