namespace Brady.Limits.ActionProcessing.Core.Tests
{
    internal class TestState : ActionProcessingState
    {
        public TestState(string currentState, object externalState = null)
            : base(currentState, externalState)
        { }
        
        public static IActionProcessingState New(string currentState, object externalState = null) => new TestState(currentState, externalState);
    }
}
