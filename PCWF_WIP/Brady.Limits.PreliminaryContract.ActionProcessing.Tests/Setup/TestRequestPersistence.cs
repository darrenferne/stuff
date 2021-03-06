﻿using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Brady.Limits.PreliminaryContract.ActionProcessing.Tests
{
    class TestRequestPersistence : IPreliminaryContractRequestPersistence
    {
        private readonly Dictionary<Guid, IActionRequest> _requestStore;
        public TestRequestPersistence()
        {
            _requestStore = new Dictionary<Guid, IActionRequest>();
        }

        public void DeletePendingRequest(Guid requestId)
        {
            if (_requestStore.ContainsKey(requestId))
                _requestStore.Remove(requestId);
        }

        public IList<IActionRequest> GetPendingRequests()
        {
            return _requestStore.Values.ToList();
        }

        void IActionProcessingRequestPersistence.SavePendingRequest<TRequest>(TRequest request)
        {
            if (_requestStore.ContainsKey(request.RequestId))
                _requestStore[request.RequestId] = request;
            else
                _requestStore.Add(request.RequestId, request);
        }
    }
}
