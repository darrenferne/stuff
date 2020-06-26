namespace Brady.Limits.ActionProcessing.Core
{
    public interface IRequestWithContext 
    {
        void InitialiseContext(IActionRequestContext state);
    }
}
