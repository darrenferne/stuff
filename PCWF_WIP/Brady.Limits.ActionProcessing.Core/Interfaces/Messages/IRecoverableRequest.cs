namespace Brady.Limits.ActionProcessing.Core
{
    public interface IRecoverableRequest : IRequest
    {
        bool IsRecoveryRequest { get; set; }
    }
}
