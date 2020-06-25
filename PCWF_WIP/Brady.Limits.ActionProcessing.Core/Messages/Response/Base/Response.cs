using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class Response : IResponse
    {
        public Response(IRequest request)
            : this(Guid.NewGuid(), string.Empty, request)
        { }

        public Response(string responseName, IRequest request)
            : this(Guid.NewGuid(), responseName, request)
        { }

        public Response(Guid responseId, string responseName, IRequest request)
        {
            ResponseName = responseName;
            Request = request;
        }

        public IRequest Request { get; }
        public string ResponseName { get; protected set; }
    }
}