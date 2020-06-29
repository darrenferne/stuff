using System;
using System.Diagnostics;

namespace Brady.Limits.ActionProcessing.Core
{
    [DebuggerDisplay("ObjectType:{ObjectType.Name}, TrackingReference:{TrackingReference}, Payload:{Object}")]
    public class ActionProcessingPayload<TPayload> : IActionRequestPayload
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