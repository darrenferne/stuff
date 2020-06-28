using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionRequestPayload 
    {
        Type ObjectType { get; }
        object Object { get; }
        Guid TrackingReference { get; }
    }
}