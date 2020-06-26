using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class StartRequest : Request
    {
        public StartRequest(Guid requestId, bool withRecovery = true)
            : base(requestId, "Start")
        {
            WithRecovery = withRecovery;
        }

        public bool WithRecovery { get; }

        public static StartRequest New(bool withRecovery = true) => new StartRequest(Guid.NewGuid(), withRecovery);
    }
}