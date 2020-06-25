using System;
using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessingRequestPersistence
    {
        IList<IActionRequest> GetPendingRequests();
        void SavePendingRequest<TRequest>(TRequest request) where TRequest : class, IActionRequest;
        void DeletePendingRequest(Guid requestId);
    }
}