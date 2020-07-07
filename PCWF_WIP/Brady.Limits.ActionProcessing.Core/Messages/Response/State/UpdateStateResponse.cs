namespace Brady.Limits.ActionProcessing.Core
{
    public class UpdateStateResponse : Response
    {
        public UpdateStateResponse(UpdateStateRequest request, IActionRequest forRequest)
            : base(request)
        {
            ForRequest = forRequest;
        } 
        public IActionRequest ForRequest { get; }
        public static UpdateStateResponse New(UpdateStateRequest request, IActionRequest forRequest) => new UpdateStateResponse(request, forRequest);
    }
}