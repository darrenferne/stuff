using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    internal class InitialiseProcessState : AllowedAction<ActionRequest<Contract>>
    {
        IPreliminaryContractStatePersistence _statePersistence;

        public InitialiseProcessState(IPreliminaryContractStatePersistence statePersistence)
            : base(nameof(InitialiseProcessState))
        {
            _statePersistence = statePersistence;
        }

        public override IActionProcessingStateChange OnInvoke(ActionRequest<Contract> request)
        {
            var contractState = _statePersistence.GetState(request.Payload.Id);

            if (contractState is null)
            {
                contractState = _statePersistence.SaveState(new ContractState());

                return new StateChange(request.Payload, ContractProcessingState.New(nameof(IsNew), contractState).WithIsNew(true));
            }
            else
                return new StateChange(request.Payload, ContractProcessingState.New(nameof(IsNotNew), contractState).WithIsNew(false));
        }
    }
}
