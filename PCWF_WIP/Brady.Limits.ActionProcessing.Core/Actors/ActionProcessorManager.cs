using Akka.Actor;
using Akka.Event;
using System;

namespace Brady.Limits.ActionProcessing.Core
{
    internal class ActionProcessorManager : ReceiveActor
    {
        public static readonly string Name = nameof(ActionManager).ToLower();

        private readonly ILoggingAdapter _log;
        private readonly IActionProcessorRequirements _requirements;

        private IActorRef _actionPipeline;
        private ActionProcessorState  _state = ActionProcessorState.Stopped;

        public ActionProcessorManager(IActionProcessorRequirements requirements)
        {
            _log = Context.GetLogger();

            _requirements = requirements;
            //TODO add validation for requirements

            Receive<StartRequest>(request => OnStart(request));
            Receive<StopRequest>(request => OnStop(request));
            Receive<IActionRequest>(request => OnProcessAction(request));
        }

        protected override void PreStart() => _log.Debug("Action processor manager started");
        protected override void PostStop() => _log.Debug("Action processor manager stopped");

        public static Props Props(IActionProcessorRequirements requirements)
        {
            return Akka.Actor.Props.Create(() => new ActionProcessorManager(requirements));
        }

        public void OnStart(StartRequest request)
        {
            var pipelineName = string.IsNullOrEmpty(_requirements.PipelineConfiguration.Name) ? ActionPipelineManager.Name : _requirements.PipelineConfiguration.Name;
            _actionPipeline = Context.ActorOf(ActionPipelineManager.Props(_requirements), pipelineName);

            if (request.WithRecovery)
                _actionPipeline.Tell(RecoveryRequest.New());

            _state = ActionProcessorState.Started;

            Sender.Tell(new Response(_state.ToString(), request));
        }

        public void OnStop(StopRequest request)
        {
            _actionPipeline
                .GracefulStop(TimeSpan.FromSeconds(10))
                .Wait();

            _state = ActionProcessorState.Stopped;

            Sender.Tell(new Response(_state.ToString(), request));
        }

        private bool ValidateStateAndAction(IActionRequest request)
        {
            var allowedStates = _requirements.PipelineConfiguration.AllowedStates;
            var currentState = request.CurrentState?.CurrentState;

            if (!allowedStates.ContainsKey(currentState) || !typeof(IExternalState).IsAssignableFrom(allowedStates[currentState].GetType()))
            {
                Sender.Tell(UnhandledResponse.New(request, $"Request Rejected. The specified current state '{request.CurrentState.CurrentState}' is not a known state."));
                return false;
            }

            if (!allowedStates[currentState].AllowedActions.ContainsKey(request.ActionName))
            {
                Sender.Tell(UnhandledResponse.New(request, $"Request Rejected. The requested action '{request.ActionName}' is not valid for the current state '{request.CurrentState.CurrentState}'."));
                return false;
            }

            return ValidateAction(request);
        }

        private bool ValidateAction(IActionRequest request)
        {
            if (!(_requirements.PipelineConfiguration.ActionTypes.ContainsKey(request.ActionName)))
            {
                Sender.Tell(UnhandledResponse.New(request, $"Request Rejected. The requested action '{request.ActionName}' is not valid."));
                return false;
            }
            else if (!typeof(IExternalAction).IsAssignableFrom(_requirements.PipelineConfiguration.ActionTypes[request.ActionName]))
            {
                Sender.Tell(UnhandledResponse.New(request, $"Request Rejected. The requested action '{request.ActionName}' is not accessible."));
                return false;
            }
            return true;
        }

        public void OnProcessAction(IActionRequest request)
        {
            //TODO add error handling and logging

            if (_state == ActionProcessorState.Started)
            {
                var canProcess = request.CurrentState is null ?
                                    ValidateAction(request) :
                                    ValidateStateAndAction(request);

                if (canProcess)
                    _actionPipeline.Forward(request);
            }
        }
    }
}
