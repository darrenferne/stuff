using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.ActionProcessing.Core.Tests
{
    class TestRequestPersistence : IActionProcessingRequestPersistence
    {
        private readonly Dictionary<Guid, IActionRequest> _requestStore;
        public TestRequestPersistence(params IActionRequest[] pendingPequests)
        {
            if (pendingPequests is null)
                _requestStore = new Dictionary<Guid, IActionRequest>();
            else
                _requestStore = new Dictionary<Guid, IActionRequest>(pendingPequests?.ToDictionary(r => r.RequestId));
        }

        public int DeleteCount;
        public void DeletePendingRequest(Guid requestId)
        {
            DeleteCount++;
            if (_requestStore.ContainsKey(requestId))
                _requestStore.Remove(requestId);
        }

        public int GetCount;
        public IList<IActionRequest> GetPendingRequests()
        {
            GetCount++;
            return _requestStore.Values.ToList();
        }

        public int SaveCount;
        void IActionProcessingRequestPersistence.SavePendingRequest<TRequest>(TRequest request)
        {
            SaveCount++;
            if (_requestStore.ContainsKey(request.RequestId))
                _requestStore[request.RequestId] = request;
            else
                _requestStore.Add(request.RequestId, request);
        }
    }
}
