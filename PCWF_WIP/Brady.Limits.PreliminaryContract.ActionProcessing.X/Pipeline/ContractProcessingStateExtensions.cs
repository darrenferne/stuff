namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public static class ContractProcessingStateExtensions
    {
        public static ContractProcessingState Clone(this ContractProcessingState state, string newCurrent = null, object newExternal = null)
        {
            return new ContractProcessingState(
                string.IsNullOrEmpty(newCurrent) ? state.CurrentState : newCurrent,
                newExternal is null ? state.ExtendedState : newExternal)
            {
                IsInFlight = state.IsInFlight,
                IsMaterialChange = state.IsMaterialChange,
                IsNew = state.IsNew,
                IsOnHold = state.IsOnHold,
                IsValid = state.IsValid
            };
        }

        public static ContractProcessingState WithIsInFlight(this ContractProcessingState state, bool isInFlight, bool isNew = true)
        {
            var newState = state.Clone();
            newState.IsInFlight = isInFlight;
            return newState;
        }
        public static ContractProcessingState WithIsMaterialChange(this ContractProcessingState state, bool isMaterialChange, bool isNew = true)
        {
            var newState = state.Clone();
            newState.IsMaterialChange = isMaterialChange;
            return newState;
        }
        public static ContractProcessingState WithIsNew(this ContractProcessingState state, bool isNew = true)
        {
            var newState = state.Clone();
            newState.IsNew = isNew;
            return newState;
        }

        public static ContractProcessingState WithIsValid (this ContractProcessingState state, bool isValid, bool isNew = true)
        {
            var newState = state.Clone();
            newState.IsValid = isValid;
            return newState;
        }
        public static ContractProcessingState WithIsOnHold(this ContractProcessingState state, bool isOnHold, bool isNew = true)
        {
            var newState = state.Clone();
            newState.IsOnHold = isOnHold;
            return newState;
        }
    }
}
