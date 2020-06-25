using System;
using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.ActionProcessing.Core
{
    public class GatedActionRequest<TPayload> : ActionRequest<TPayload>, IContinuationRequest
        where TPayload : IActionProcessingPayload
    {
        public GatedActionRequest(Guid requestId, string actionName, TPayload payload, params GateDescriptor[] gates)
            : base(requestId, actionName, payload)
        {
            Gates = gates.ToDictionary(g => g.State, g => g.TriggerAction);
        }

        public GatedActionRequest(Guid requestId, string actionName, TPayload payload, IDictionary<string, ActionRequestDescriptor> gates)
            : base(requestId, actionName, payload)
        {
            Gates = gates;
        }

        public GatedActionRequest(string actionName, TPayload payload, params GateDescriptor[] gates)
            : this(Guid.NewGuid(), actionName, payload, gates)
        { }

        public GatedActionRequest(string actionName, TPayload payload, IDictionary<string, ActionRequestDescriptor> gates)
            : this(Guid.NewGuid(), actionName, payload, gates)
        { }

        public IDictionary<string, ActionRequestDescriptor> Gates { get; }

        public ActionRequestDescriptor NextRequest(IActionProcessingState state)
        {
            return Gates.ContainsKey(state.CurrentState) ? Gates[state.CurrentState] : null;
        }
    }
}