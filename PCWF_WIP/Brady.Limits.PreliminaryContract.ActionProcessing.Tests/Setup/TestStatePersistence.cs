using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;
using System;
using System.Collections.Generic;

namespace Brady.Limits.PreliminaryContract.ActionProcessing.Tests
{
    class TestStatePersistence : IPreliminaryContractStatePersistence
    {
        private readonly Func<IActionRequest, (string, object)> _getInitialState;
        private readonly Dictionary<Guid, IActionProcessingState> _stateStore;
        public TestStatePersistence(Func<IActionRequest, (string, object)> getInitialState)
        {
            _getInitialState = getInitialState;
            _stateStore = new Dictionary<Guid, IActionProcessingState>();
        }

        public IActionProcessingState GetCurrentState(IActionRequest request)
        {
            if (_stateStore.ContainsKey(request.Payload.TrackingReference))
            {
                var state = _stateStore[request.Payload.TrackingReference] as ContractProcessingState;
                return state.WithIsNew(false); 
            }
            else
                return null;
        }

        public IActionProcessingState GetInitialState(IActionRequest request)
        {
            (string current, object external) = _getInitialState(request);

            return new ContractProcessingState(current, external);
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
