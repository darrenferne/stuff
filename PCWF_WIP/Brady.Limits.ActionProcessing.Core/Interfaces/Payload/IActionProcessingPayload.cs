using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessingPayload 
    {
        Type ObjectType { get; }
        object Object { get; }
        Guid TrackingReference { get; }
    }
}