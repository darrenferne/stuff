using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core
{
    public class ActionManager : ReceiveActor
    {
        public static readonly string Name = nameof(ActionManager).ToLower();

        private readonly ILoggingAdapter _log;
        private readonly IActionProcessorRequirements _requirements;
        private readonly IAllowedState _state;
        private readonly IDictionary<string, IAllowedAction> _allowedActions;

        public ActionManager(IActionProcessorRequirements requirements, IAllowedState forState)
        {
            _log = Context.GetLogger();

            _requirements = requirements;
            _state = forState;

            _allowedActions = new Dictionary<string, IAllowedAction>(_state.AllowedActions);

            Receive<IActionRequest>(request => OnReceiveRequest(request),
                                    request => CanHandleRequest(request));

            Receive<IActionRequest>(request => OnUnhandledRequest(request));

        }

        protected override void PreStart() => _log.Debug("Action manager started");
        protected override void PostStop() => _log.Debug("Action manager stopped");

        public static Props Props(IActionProcessorRequirements requirements, IAllowedState forState)
        {
            return Akka.Actor.Props.Create(() => new ActionManager(requirements, forState));
        }

        public IAllowedState ForState => _state;
        
        private bool CanHandleRequest(IActionRequest request)
        {
            //TODO add error handling and logging
            var canHandle = _allowedActions.ContainsKey(request.ActionName);
            return canHandle;
        }

        public void OnReceiveRequest(IActionRequest request)
        {
            //TODO add error handling and logging

            var actionType = _requirements.PipelineConfiguration.ActionTypes[request.ActionName];
            var action = _allowedActions[request.ActionName];

            if (action is null)
            {
                action = _requirements.PipelineConfiguration.CreateAction(actionType);
                _allowedActions[request.ActionName] = action;
            }

            try
            {
                var stateChange = action.Invoke(request);
                var response = new ActionResponse(request, stateChange);

                Context.Parent.Tell(response);
            }
            catch (Exception ex)
            {
                _log.Error($"Failed to process action '{request.ActionName}'. RequestName: {request.RequestName}, RequestId: {request.RequestId}), Details: {ex.Message}");
            }
        }

        public void OnUnhandledRequest(IActionRequest request)
        {
            Sender.Tell(UnhandledResponse.New(request, $"Request Rejected. The requested action '{request.ActionName}' is not valid for the current state: '{request.Context.CurrentState.StateName}'."));
        }
    }
}
