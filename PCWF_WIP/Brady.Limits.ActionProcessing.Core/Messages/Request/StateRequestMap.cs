using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class StateRequestMap
    {
        public StateRequestMap(string state, Func<string, IActionRequest> requestProvider)
        { 
            State = state;
            RequestProvider = requestProvider;
        }

        public string State { get; }
        public Func<string, IActionRequest> RequestProvider { get; }
    }
}
