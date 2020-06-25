namespace Brady.Limits.ActionProcessing.Core
{
    public class GateDescriptor
    {
        public GateDescriptor(string state, ActionRequestDescriptor triggerAction)
        {
            State = state;
            TriggerAction = triggerAction;
        }
        public string State { get; }
        public ActionRequestDescriptor TriggerAction { get; }
    }
}