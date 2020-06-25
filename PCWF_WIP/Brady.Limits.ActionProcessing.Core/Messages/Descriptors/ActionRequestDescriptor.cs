using System;
using System.Diagnostics;
using System.Reflection;

namespace Brady.Limits.ActionProcessing.Core
{
    [DebuggerDisplay("RequestType:{RequestType}, ActionName:{ActionName}")]

    public class ActionRequestDescriptor
    {
        public ActionRequestDescriptor(Type requestType)
        {
            RequestType = requestType;
        }
        public ActionRequestDescriptor(string actionName)
            : this(typeof(ActionRequest<>))
        {
            ActionName = actionName;
        }
        public Type RequestType { get; set; }
        public string ActionName { get; set; }

        protected IActionRequest ConstructRequest(Type requestType, object payload, IActionProcessingState state)
        {
            foreach(var constructor in requestType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                switch(parameters.Length)
                {
                    case 2:
                        if (parameters[0].ParameterType.IsAssignableFrom(state.GetType()) &&
                            parameters[1].ParameterType.IsAssignableFrom(payload.GetType()))
                        {
                            return Activator.CreateInstance(requestType, state, payload) as IActionRequest;
                        }
                        break;
                    case 3:
                        if (parameters[0].ParameterType == typeof(string) && 
                            parameters[1].ParameterType.IsAssignableFrom(state.GetType()) &&
                            parameters[2].ParameterType.IsAssignableFrom(payload.GetType()))
                        {
                            return Activator.CreateInstance(requestType, ActionName, state, payload) as IActionRequest;
                        }
                        break;
                }
            }
            
            throw new ActionProcessorException($"Could not find a suitable constructor for type: {requestType}");
        }

        public virtual IActionRequest ToRequest(Type payloadType, object payload, IActionProcessingState state)
        {
            if (RequestType.IsGenericType)
            {
                var genericType = RequestType.MakeGenericType(payloadType);

                return ConstructRequest(genericType, payload, state);
            }
            else
            {
                var payloadProperty = RequestType.GetProperty(nameof(IActionRequest.Payload), BindingFlags.Public | BindingFlags.Instance);
                if (payloadType.IsAssignableFrom(payloadProperty.PropertyType))
                    return ConstructRequest(RequestType, payload, state);

                throw new ActionProcessorException($"The request payload type is incompatible with the given action request descriptor '{RequestType.Name}'.");
            }
        }
    }
}
