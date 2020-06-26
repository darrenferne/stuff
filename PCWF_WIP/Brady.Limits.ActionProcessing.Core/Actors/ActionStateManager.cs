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

            Receive<GetStateRequest>(request => OnRestoreStateRequest(request));
            Receive<UpdateStateRequest>(request => OnUpdateStateRequest(request));
        }

        protected override void PreStart() => _log.Debug("Action state manager started");
        protected override void PostStop() => _log.Debug("Action state manager stopped");

        public static Props Props(IActionProcessorRequirements requirements)
        {
            return Akka.Actor.Props.Create(() => new ActionStateManager(requirements));
        }
                
        public void OnRestoreStateRequest(GetStateRequest request)
        {
            var currentState = _requirements.StatePersistence.GetCurrentState(request.ForRequest);
            if (currentState is null)
                currentState = _requirements.StatePersistence.GetInitialState(request.ForRequest);

            Sender.Tell(GetStateResponse.New(request, currentState));
        }

        public void OnUpdateStateRequest(UpdateStateRequest request)
        {
           _requirements.StatePersistence.SetCurrentState(request.ForRequest, request.ForRequestResponse.StateChange.NewState);
            
            Sender.Tell(UpdateStateResponse.New(request, request.ForRequest));
        }
    }
}
