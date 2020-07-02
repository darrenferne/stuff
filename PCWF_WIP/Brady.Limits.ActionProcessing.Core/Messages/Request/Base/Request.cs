using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class Request : IRequest
    {
        public Request(Guid requestId, string name)
        {
            RequestId = requestId;
            RequestName = name;
            RequestType = this.GetType();

        }
        public Type RequestType{ get; }
        public Guid RequestId { get; }
        public string RequestName { get; }
    }
}