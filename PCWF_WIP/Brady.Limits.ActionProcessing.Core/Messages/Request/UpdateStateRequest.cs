using System;

namespace Brady.Limits.ActionProcessing.Core
{
    public class UpdateStateRequest : Request
    {
        public UpdateStateRequest(Guid requestId, IActionResponse response)
            : base(requestId, "UpdateState")
        {
            Response = response;
        }

        public IActionResponse Response { get; }
        public static UpdateStateRequest New(IActionResponse response) => new UpdateStateRequest(Guid.NewGuid(), response);
    }
}