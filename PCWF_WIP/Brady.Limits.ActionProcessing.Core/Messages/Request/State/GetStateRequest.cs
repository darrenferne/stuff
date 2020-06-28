using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class GetStateRequest : Request
    {
        public GetStateRequest(Guid requestId, IRequestWithState forRequest)
            : base(requestId, "GetState")
        {
            ForRequest = forRequest;
        }

        public IRequestWithState ForRequest { get; }
        public static GetStateRequest New(IRequestWithState forRequest) => new GetStateRequest(Guid.NewGuid(), forRequest);
    }
}