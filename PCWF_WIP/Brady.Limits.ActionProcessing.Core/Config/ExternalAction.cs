namespace Brady.Limits.ActionProcessing.Core
{
    public abstract class ExternalAction<TRequest, TResult> : AllowedAction<TRequest, TResult>, IExternalAction
        where TRequest : class, IActionRequest
        where TResult : class, IActionResult
    {
        public ExternalAction()
            : base()
        { }
        public ExternalAction(string name)
            : base(name)
        { }
    }
    
}
