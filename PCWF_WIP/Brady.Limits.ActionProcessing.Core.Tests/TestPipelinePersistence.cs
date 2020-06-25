using System;
using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core.Tests
{
    class TestPipelinePersistence : IActionPipelinePersistence
    {
        Dictionary<Guid, IActionRequest> _store;

        public TestPipelinePersistence()
        {
            _store = new Dictionary<Guid, IActionRequest>();
        }
        public void DeletePendingRequest(Guid requestId)
        {
            if (_store.ContainsKey(requestId))
                _store.Remove(requestId);
        }

        public IList<IActionRequest> GetPendingRequests()
        {
            return new List<IActionRequest>(_store.Values);
        }

        public void SavePendingRequest<TRequest>(TRequest request) where TRequest : class, IActionRequest
        {
            _store.Add(request.RequestId, request);
        }
    }
}
