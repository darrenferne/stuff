using System;
using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.ActionProcessing.Core
{
    public class GatedRequestDescriptor : ActionRequestDescriptor
    {
        public GatedRequestDescriptor(string actionName, params GateDescriptor[] gates)
            : base(actionName)
        {
            GateDescriptors = gates.ToDictionary(g => g.State, g => g.TriggerAction);
        }

        public IDictionary<string, ActionRequestDescriptor> GateDescriptors { get; }

        private IActionRequest ConstructRequest(Type requestType, string actionName, object payload, IActionProcessingState state, IDictionary<string, ActionRequestDescriptor> gates)
        {
            foreach (var constructor in requestType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                switch (parameters.Length)
                {
                    case 3:
                        if (parameters[0].ParameterType.IsAssignableFrom(state.GetType()) &&
                            parameters[1].ParameterType.IsAssignableFrom(payload.GetType()) &&
                            parameters[2].ParameterType.IsAssignableFrom(gates.GetType()))
                        {
                            return Activator.CreateInstance(requestType, state, payload, gates) as IActionRequest;
                        }
                        break;
                    case 4:
                        if (parameters[0].ParameterType == typeof(string) &&
                            parameters[1].ParameterType.IsAssignableFrom(state.GetType()) &&
                            parameters[2].ParameterType.IsAssignableFrom(payload.GetType()) &&
                            parameters[1].ParameterType.IsAssignableFrom(gates.GetType()))
                        {
                            return Activator.CreateInstance(requestType, ActionName, state, payload, gates) as IActionRequest;
                        }
                        break;
                }
            }

            return base.ConstructRequest(requestType, payload, state);
        }

        public override IActionRequest ToRequest(Type payloadType, object payload, IActionProcessingState state)
        {
            var requestType = typeof(GatedActionRequest<>);
            var genericType = requestType.MakeGenericType(payloadType);

            return Activator.CreateInstance(genericType, ActionName, state, payload, GateDescriptors) as IActionRequest;
        }
    }
}
