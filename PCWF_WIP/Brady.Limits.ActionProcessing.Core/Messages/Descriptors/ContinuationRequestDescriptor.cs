using System;
using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.ActionProcessing.Core
{
    public class ContinuationRequestDescriptor : ActionRequestDescriptor
    {
        public ContinuationRequestDescriptor(string actionName, params ActionRequestDescriptor[] continuations)
            : this(actionName, new List<ActionRequestDescriptor>(continuations ?? Enumerable.Empty<ActionRequestDescriptor>()))
        { }
        public ContinuationRequestDescriptor(string actionName, IList<ActionRequestDescriptor> continuations)
            : base(actionName)
        {
            Continuations = new List<ActionRequestDescriptor>(continuations);
        }

        public IEnumerable<ActionRequestDescriptor> Continuations { get; }
    
        private IActionRequest ConstructRequest(Type requestType, string actionName, IActionRequestPayload payload, List<ActionRequestDescriptor> continuations)
        {
            foreach (var constructor in requestType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                switch (parameters.Length)
                {
                    case 2:
                        if (parameters[0].ParameterType.IsAssignableFrom(payload.GetType()) &&
                            parameters[1].ParameterType.IsAssignableFrom(continuations.GetType()))
                        {
                            return Activator.CreateInstance(requestType, payload, continuations) as IActionRequest;
                        }
                        break;
                    case 3:
                        if (parameters[0].ParameterType == typeof(string) &&
                            parameters[1].ParameterType.IsAssignableFrom(payload.GetType()) &&
                            parameters[2].ParameterType.IsAssignableFrom(continuations.GetType()))
                        {
                            return Activator.CreateInstance(requestType, ActionName, payload, continuations) as IActionRequest;
                        }
                        break;
                }
            }

            return base.ConstructRequest(requestType, payload);
        }

        public override IActionRequest ToRequest(Type payloadType, IActionRequestPayload payload)
        {
            var requestType = typeof(ContinuationActionRequest<>);
            var genericType = requestType.MakeGenericType(payloadType);

            return ConstructRequest(genericType, ActionName, payload, Continuations.ToList()) as IActionRequest;
        }
    }
}
