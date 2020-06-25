using System;
using System.Diagnostics;

namespace Brady.Limits.ActionProcessing.Core
{
    [DebuggerDisplay("ObjectType:{ActionName}, TrackingReference:{}, Payload:{CurrentState.Name}")]
    public class ActionProcessingPayload<TPayload> : IActionProcessingPayload
    {
        public ActionProcessingPayload(TPayload payload, Guid? trackingReference = null)
        {
            Object = payload;
            TrackingReference = trackingReference ?? Guid.NewGuid();
        }
        public Type ObjectType { get; } = typeof(TPayload);

        public object Object { get; }

        public Guid TrackingReference { get; }
    }
}