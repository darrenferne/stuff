using Akka.Actor;
using Akka.Event;
using System.Threading.Tasks;

namespace Brady.Limits.ActionProcessing.Core
{
    public class ActionProcessor : IActionProcessor
    {
        private readonly ILoggingAdapter _log;
        private readonly IActionProcessorRequirements _requirements;

        private IActorRef _actionProcessorManager;
        
        public ActionProcessor(IActionProcessorRequirements requirements, ActorSystem actorSystem)
        {
            _log = actorSystem.Log;
            _requirements = requirements;

            var name = string.IsNullOrEmpty(_requirements.PipelineConfiguration.Name) ? $"{ActionProcessorManager.Name}" : $"{ActionProcessorManager.Name}_{_requirements.PipelineConfiguration.Name}";
            _actionProcessorManager = actorSystem.ActorOf(ActionProcessorManager.Props(_requirements), name);
        }

        public ActionProcessorState State { get; internal set; }
        public void Start(bool withRecovery = true)
        {
            _actionProcessorManager
                .Ask(StartRequest.New(withRecovery))
                .ContinueWith(t =>
                {
                    State = ActionProcessorState.Started;
                })
                .Wait();
        }

        public void Stop()
        {
            _actionProcessorManager
                .Ask(StopRequest.New())
                .ContinueWith(t =>
                {
                    State = ActionProcessorState.Stopped;
                })
                .Wait();
        }

        public void ProcessAction<TRequest>(TRequest request, string userName = null)
            where TRequest : class, IActionRequest
        {
            if (State != ActionProcessorState.Started)
                throw new ActionProcessorException("ProcessAction requests cannot be submitted before the processor has been started");

            var requestWithContext = request as IRequestWithContext;
            if (!(requestWithContext is null))
            {
                var user = new ActionProcessorUser(_requirements.Authorisation, userName);
                var context = new ActionRequestContext(user, _actionProcessorManager, request, null, null);

                requestWithContext.InitialiseContext(context);
            }

            _actionProcessorManager.Tell(request);
        }

        public Task<IActionResponse> ProcessActionAsync<TRequest>(TRequest request, string userName = null)
            where TRequest : class, IRequestWithContext
        {
            if (State != ActionProcessorState.Started)
                throw new ActionProcessorException("ProcessAction requests cannot be submitted before the processor has been started");

            var user = new ActionProcessorUser(_requirements.Authorisation, userName);
            var completionSource = new TaskCompletionSource<IActionResponse>();
            var context = new ActionRequestContext(user, _actionProcessorManager, request as IActionRequest, null, completionSource);

            request.InitialiseContext(context);
            _actionProcessorManager.Tell(request);

            return completionSource.Task;
        }
    }
}
