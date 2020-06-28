namespace Brady.Limits.ActionProcessing.Core
{
    public class ActionProcessingState : IActionProcessingState
    {
        public ActionProcessingState(string currentState, object externalState = null)
        {
            StateName = currentState;
            ExtendedState = externalState;
        }
        public string StateName { get; }

        public object ExtendedState { get; }

        public bool Equals(IActionProcessingState other)
        {
            return
                !(other is null) 
                &&
                (string.IsNullOrEmpty(StateName) && string.IsNullOrEmpty(other.StateName) || (StateName?.Equals(other.StateName) ?? false))
                &&
                (ExtendedState is null && other.ExtendedState is null || (ExtendedState?.Equals(other.ExtendedState) ?? false));
        }
    }
}
