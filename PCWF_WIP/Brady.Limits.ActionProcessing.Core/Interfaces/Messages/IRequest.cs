using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IRequest
    {
        Guid RequestId { get; }
        string RequestName { get; }
    }
}
