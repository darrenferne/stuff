using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionRequest : IRecoverableRequest
    {
        string ActionName { get; }
        IActionProcessingState CurrentState { get; }
        Type PayloadType { get;}
        IActionProcessingPayload Payload { get; }
    }
}
