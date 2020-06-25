using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class RestoreStateRequest : Request
    {
        public RestoreStateRequest(Guid requestId, IStatePersistentRequest request)
            : base(requestId, "RestoreState")
        {
            Request = request;
        }

        public IStatePersistentRequest Request { get; }
        public static RestoreStateRequest New(IStatePersistentRequest request) => new RestoreStateRequest(Guid.NewGuid(), request);
    }
}