using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class RecoveryRequest : Request
    {
        public RecoveryRequest(Guid requestId)
            : base(requestId, "Recovery")
        { }

        public static RecoveryRequest New() => new RecoveryRequest(Guid.NewGuid());
    }
}