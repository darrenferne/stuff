namespace Brady.Limits.ActionProcessing.Core
{
    internal static class ActionRequestExtensions
    {
        public static IActionRequest FlagForRecovery(this IActionRequest request)
        {
            request.IsRecoveryRequest = true;
            return request;
        }
    }
}
