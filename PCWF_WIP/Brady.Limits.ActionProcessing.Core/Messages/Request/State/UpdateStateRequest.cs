using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class UpdateStateRequest : Request
    {
        public UpdateStateRequest(Guid requestId, IActionRequest forRequest, IActionResponse forRequestResponse)
            : base(requestId, "UpdateState")
        {
            ForRequest = forRequest;
            ForRequestResponse = forRequestResponse;
        }

        public IActionRequest ForRequest { get; }
        public IActionResponse ForRequestResponse { get; }
        public static UpdateStateRequest New(IActionRequest forRequest, IActionResponse forRequestResponse) => new UpdateStateRequest(Guid.NewGuid(), forRequest, forRequestResponse);
    }
}