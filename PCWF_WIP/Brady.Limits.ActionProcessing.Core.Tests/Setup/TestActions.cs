namespace Brady.Limits.ActionProcessing.Core.Tests
{
    public class Add : AllowedAction<ActionRequest<IntegerPayload>>, IExternalAction
    {
        public Add(TestActionIoC injected)
        {
            Injected = injected;
        }
        public TestActionIoC Injected { get; }
        public override IActionResult OnInvoke(ActionRequest<IntegerPayload> request)
        {
            var payload = request.Payload as IntegerPayload;
            var newPayload = IntegerPayload.New(payload.Value + 1);
            var newState = TestState.New(newPayload.Value > 0 ? "Positive" : newPayload.Value < 0 ? "Negative" : "Zero");
            var stateChange = new StateChangeResult(newPayload, newState, string.Empty);
            
            Injected.SavePayload(newPayload);

            return stateChange;
        }
    }
}
