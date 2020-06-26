using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class UnhandledResponse : Response
    {
        public UnhandledResponse(IRequest request, string message)
            : this(Guid.NewGuid(), request, message)
        { }

        public UnhandledResponse(Guid responseId, IRequest request, string message)
            : base(responseId, "Unhandled", request)
        {
            Message = message;
        }

        public string Message { get; }
        
        public static UnhandledResponse New(IRequest request, string message) => new UnhandledResponse(request, message);
    }
}
