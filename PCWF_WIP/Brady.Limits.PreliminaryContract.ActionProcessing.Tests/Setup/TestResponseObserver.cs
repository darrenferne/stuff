using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;

namespace Brady.Limits.PreliminaryContract.ActionProcessing.Tests
{
    class TestResponseObserver : IActionResponseObserver
    {
        public List<IActionResponse> Responses { get; } = new List<IActionResponse>();
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(IActionResponse value)
        {
            Responses.Add(value);
        }
    }
}
