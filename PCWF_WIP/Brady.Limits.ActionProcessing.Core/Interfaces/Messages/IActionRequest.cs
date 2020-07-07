using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionRequest<out TPayload> : IActionRequest
        where TPayload : IActionRequestPayload
    {
        new TPayload Payload { get; }
    }

    public interface IActionRequest : IRecoverableRequest
    {
        string ActionName { get; }
        IActionRequestContext Context { get; }
        Type PayloadType { get;}
        IActionRequestPayload Payload { get; }
    }
}
