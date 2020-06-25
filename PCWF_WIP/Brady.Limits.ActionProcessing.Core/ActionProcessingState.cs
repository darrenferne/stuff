namespace Brady.Limits.ActionProcessing.Core
{
    public class ActionProcessingState : IActionProcessingState
    {
        public ActionProcessingState(string currentState, object externalState = null)
        {
            CurrentState = currentState;
            ExternalState = externalState;
        }
        public string CurrentState { get; }

        public object ExternalState { get; }

        public bool Equals(IActionProcessingState other)
        {
            return
                !(other is null) 
                &&
                (string.IsNullOrEmpty(CurrentState) && string.IsNullOrEmpty(other.CurrentState) || (CurrentState?.Equals(other.CurrentState) ?? false))
                &&
                (ExternalState is null && other.ExternalState is null || (ExternalState?.Equals(other.ExternalState) ?? false));
        }
    }
}
