using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class GetStateRequest : Request
    {
        public GetStateRequest(Guid requestId, IStatePersistentRequest forRequest)
            : base(requestId, "GetState")
        {
            ForRequest = forRequest;
        }

        public IStatePersistentRequest ForRequest { get; }
        public static GetStateRequest New(IStatePersistentRequest forRequest) => new GetStateRequest(Guid.NewGuid(), forRequest);
    }
}