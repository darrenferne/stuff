using System.Threading.Tasks;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessor
    {
        ActionProcessorState State { get; }
        void Start(bool withRecovery = true);
        void Stop();
        void ProcessAction<TRequest>(TRequest request, string userName = null)
            where TRequest : class, IActionRequest;

        Task<IActionResponse> ProcessActionAsync<TRequest>(TRequest request, string userName = null)
            where TRequest : class, IRequestWithContext;
    }
}