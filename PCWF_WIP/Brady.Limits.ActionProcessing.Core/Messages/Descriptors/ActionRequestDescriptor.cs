using System;
using System.Diagnostics;
using System.Reflection;

namespace Brady.Limits.ActionProcessing.Core
{
    [DebuggerDisplay("RequestType:{RequestType.Name}, ActionName:{ActionName}")]

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

        protected IActionRequest ConstructRequest(Type requestType, IActionRequestPayload payload)
        {
            foreach(var constructor in requestType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                switch(parameters.Length)
                {
                    case 1:
                        if (parameters[0].ParameterType.IsAssignableFrom(payload.GetType()))
                        {
                            return Activator.CreateInstance(requestType, payload) as IActionRequest;
                        }
                        break;
                    case 2:
                        if (parameters[0].ParameterType == typeof(string) && 
                            parameters[1].ParameterType.IsAssignableFrom(payload.GetType()))
                        {
                            return Activator.CreateInstance(requestType, ActionName, payload) as IActionRequest;
                        }
                        break;
                }
            }
            
            throw new ActionProcessorException($"Could not find a suitable constructor for type: {requestType.Name}");
        }

        public virtual IActionRequest ToRequest(Type payloadType, IActionRequestPayload payload)
        {
            if (RequestType.IsGenericType)
            {
                var genericType = RequestType.MakeGenericType(payloadType);

                return ConstructRequest(genericType, payload);
            }
            else
            {
                var payloadProperty = RequestType.GetProperty(nameof(IActionRequest.Payload), BindingFlags.Public | BindingFlags.Instance);
                if (payloadProperty.PropertyType.IsAssignableFrom(payloadType))
                    return ConstructRequest(RequestType, payload);

                throw new ActionProcessorException($"The request payload type is incompatible with the given action request descriptor '{RequestType.Name}'.");
            }
        }
    }
}
