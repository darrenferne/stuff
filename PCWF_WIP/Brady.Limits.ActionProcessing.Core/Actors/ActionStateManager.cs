using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core
{
    public class ActionStateManager : ReceiveActor
    {
        public static readonly string Name = nameof(ActionStateManager).ToLower();

        private readonly ILoggingAdapter _log;
        private readonly IActionProcessorRequirements _requirements;
        
        public ActionStateManager(IActionProcessorRequirements requirements)
        {
            _log = Context.GetLogger();

            _requirements = requirements;

            Receive<RestoreStateRequest>(request => OnRestoreStateRequest(request));
            Receive<UpdateStateRequest>(request => OnUpdateStateRequest(request));
        }

        protected override void PreStart() => _log.Debug("Action state manager started");
        protected override void PostStop() => _log.Debug("Action state manager stopped");

        public static Props Props(IActionProcessorRequirements requirements)
        {
            return Akka.Actor.Props.Create(() => new ActionStateManager(requirements));
        }
                
        public void OnRestoreStateRequest(RestoreStateRequest request)
        {
            var currentState = _requirements.StatePersistence.GetCurrentState(request.Request);
            if (currentState is null)
                currentState = _requirements.StatePersistence.GetInitialState(request.Request);

            request.Request.SetState(currentState);

            Sender.Forward(request.Request);
        }

        public void OnUpdateStateRequest(UpdateStateRequest request)
        {
            var originalRequest = request.Response.Request as IActionRequest;
            var newState = request.Response.StateChange.NewState;

            _requirements.StatePersistence.SetCurrentState(originalRequest, newState);
            
            Sender.Forward(originalRequest);
        }
    }
}
