namespace Brady.Limits.ActionProcessing.Core
{
    public class ActionProcessingState : IActionProcessingState
    {
        public ActionProcessingState(string currentState, object externalState = null)
        {
            CurrentState = currentState;
            ExtendedState = externalState;
        }
        public string CurrentState { get; }

        public object ExtendedState { get; }

        public bool Equals(IActionProcessingState other)
        {
            return
                !(other is null) 
                &&
                (string.IsNullOrEmpty(CurrentState) && string.IsNullOrEmpty(other.CurrentState) || (CurrentState?.Equals(other.CurrentState) ?? false))
                &&
                (ExtendedState is null && other.ExtendedState is null || (ExtendedState?.Equals(other.ExtendedState) ?? false));
        }
    }
}
