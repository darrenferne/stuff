namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public static class ContractProcessingStateExtensions
    {
        public static ContractState Clone(this ContractState state)
        {
            return new ContractState
            {
                IsInflight = state.IsInflight,
                IsMaterialChange = state .IsMaterialChange,
                IsNew = state.IsNew,
                IsOnHold = state.IsOnHold,
                IsValid = state.IsValid
            };
        }

        public static ContractState WithIsInflight(this ContractState state, bool isInflight = true)
        {
            var newState = state.Clone();
            newState.IsInflight = isInflight;
            return newState;
        }
        public static ContractState WithIsMaterialChange(this ContractState state, bool isMaterialChange = true)
        {
            var newState = state.Clone();
            newState.IsMaterialChange = isMaterialChange;
            return newState;
        }
        public static ContractState WithIsNew(this ContractState state, bool isNew = true)
        {
            var newState = state.Clone();
            newState.IsNew = isNew;
            return newState;
        }

        public static ContractState WithIsValid(this ContractState state, bool isValid = true)
        {
            var newState = state.Clone();
            newState.IsValid = isValid;
            return newState;
        }
        public static ContractState WithIsOnHold(this ContractState state, bool isOnHold = true)
        {
            var newState = state.Clone();
            newState.IsOnHold = isOnHold;
            return newState;
        }

        public static ContractProcessingState Clone(this ContractProcessingState state, string newCurrentState = null, ContractState newContractState = null)
        {
            return new ContractProcessingState(
                string.IsNullOrEmpty(newCurrentState) ? state.StateName : newCurrentState,
                newContractState is null ? state.ContractState.Clone() : newContractState);
        }

        public static ContractProcessingState WithIsInflight(this ContractProcessingState state, bool? isInflight = true)
        {
            var contractState = isInflight.HasValue ? state.ContractState.Clone().WithIsInflight(isInflight.Value) : state.ContractState.Clone();
            var currentState = contractState.IsInflight.GetValueOrDefault() ? nameof(IsInflight) : nameof(IsNotInflight);

            return state.Clone(currentState, contractState);
        }

        public static ContractProcessingState WithIsMaterialChange(this ContractProcessingState state, bool? isMaterialChange = true)
        {
            var contractState = isMaterialChange.HasValue ? state.ContractState.Clone().WithIsMaterialChange(isMaterialChange.Value) : state.ContractState.Clone();
            var currentState = contractState.IsMaterialChange.GetValueOrDefault() ? nameof(IsMaterialChange) : nameof(IsNotMaterialChange);

            return state.Clone(currentState, contractState);
        }
        public static ContractProcessingState WithIsNew(this ContractProcessingState state, bool? isNew = true)
        {
            var contractState = isNew.HasValue ? state.ContractState.Clone().WithIsNew(isNew.Value) : state.ContractState.Clone();
            var currentState = contractState.IsNew.GetValueOrDefault() ? nameof(IsNew) : nameof(IsNotNew);

            return state.Clone(currentState, contractState);
        }
               
        public static ContractProcessingState WithIsValid (this ContractProcessingState state, bool? isValid = true)
        {
            var contractState = isValid.HasValue ? state.ContractState.Clone().WithIsValid(isValid.Value) : state.ContractState.Clone();
            var currentState = contractState.IsValid.GetValueOrDefault() ? nameof(isValid) : nameof(IsNotValid);

            return state.Clone(currentState, contractState);
        }
        public static ContractProcessingState WithIsOnHold(this ContractProcessingState state, bool? isOnHold = true)
        {
            var contractState = isOnHold.HasValue ? state.ContractState.Clone().WithIsOnHold(isOnHold.Value) : state.ContractState.Clone();
            var currentState = contractState.IsOnHold.GetValueOrDefault() ? nameof(IsOnHold) : nameof(IsNotOnHold);

            return state.Clone(currentState, contractState);
        }
    }
}
