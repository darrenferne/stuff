namespace Brady.Limits.ActionProcessing.Core
{
    public static class ActionResultExtensions
    {
        public static IActionProcessingStateChange GetStateChange(this IActionResult result) => result as IActionProcessingStateChange;
        public static IActionProcessingStateChange GetStateChange(this IResponse response) => (response as IActionResponse)?.Result as IActionProcessingStateChange;
        public static IActionProcessingState GetNewState(this IResponse response) => ((response as IActionResponse)?.Result as IActionProcessingStateChange)?.NewState;
        public static TState GetNewState<TState>(this IResponse response)
            where TState : class, IActionProcessingState
        {
            return response.GetNewState() as TState;
        }
    }
}
