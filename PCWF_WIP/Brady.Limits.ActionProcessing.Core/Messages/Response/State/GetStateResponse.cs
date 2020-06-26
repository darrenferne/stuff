using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class GetStateResponse : Response
    { 
        public GetStateResponse(GetStateRequest request, IActionRequest forRequest, IActionProcessingState currentState)
            : base(request)
        {
            ForRequest = forRequest;
            CurrentState = currentState;
        }
        public IActionRequest ForRequest { get; }
        public IActionProcessingState CurrentState { get; }
        public static GetStateResponse New(GetStateRequest request, IActionProcessingState currentState) => new GetStateResponse(request, request.ForRequest, currentState);
        public static GetStateResponse New(GetStateRequest request, IActionRequest forRequest, IActionProcessingState currentState) => new GetStateResponse(request, forRequest, currentState);
    }
}