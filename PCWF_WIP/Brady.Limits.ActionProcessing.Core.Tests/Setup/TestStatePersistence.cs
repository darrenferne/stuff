﻿using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core.Tests
{
    class TestStatePersistence : IActionProcessingStatePersistence
    {
        private readonly Dictionary<Guid, IActionProcessingState> _stateStore;
        public TestStatePersistence()
        {
            _stateStore = new Dictionary<Guid, IActionProcessingState>();
        }
        public IActionProcessingState GetCurrentState(IActionRequest request)
        {
            if (_stateStore.ContainsKey(request.Payload.TrackingReference))
                return _stateStore[request.Payload.TrackingReference];
            else
                return null;
        }

        public IActionProcessingState GetInitialState(IActionRequest request)
        {
            return new ActionProcessingState("IsNew");
        }

        public void SetCurrentState(IActionRequest request, IActionProcessingState newState)
        {
            if (_stateStore.ContainsKey(request.Payload.TrackingReference))
                _stateStore[request.Payload.TrackingReference] = newState;
            else
                _stateStore.Add(request.Payload.TrackingReference, newState);
        }
    }
}
