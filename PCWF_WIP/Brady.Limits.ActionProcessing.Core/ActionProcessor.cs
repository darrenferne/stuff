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

        public Task<IResponse> ProcessAction<TRequest>(TRequest request)
            where TRequest : class, IActionRequest
        {
            if (State != ActionProcessorState.Started)
                throw new ActionProcessorException("ProcessAction requests cannot be submitted before the processor has been started");

            return  _actionProcessorManager
                .Ask(request)
                .ContinueWith(t => t.Result as IResponse);
        }        
    }
}
