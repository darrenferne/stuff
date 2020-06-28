using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class StopRequest : Request
    {
        public StopRequest(Guid requestId)
            : base(requestId, "Stop")
        { }
        
        public static StopRequest New() => new StopRequest(Guid.NewGuid());
    }
}