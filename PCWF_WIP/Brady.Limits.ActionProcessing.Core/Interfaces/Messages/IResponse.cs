namespace Brady.Limits.ActionProcessing.Core
{
    public interface IResponse
    {
        IRequest Request { get; }
        string ResponseName { get; }
    }
}