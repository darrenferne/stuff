using Akka.Actor;
using Akka.Event;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Brady.Limits.ActionProcessing.Core
{
    internal class ActionPipelineManager : ReceiveActor
    {
        public static readonly string Name = nameof(ActionPipelineManager).ToLower();

        private readonly ILoggingAdapter _log;
        protected readonly IActionProcessorRequirements _requirements;
        
        protected readonly ConcurrentDictionary<string, IActorRef> _stateProcessors;
        protected readonly IActorRef _stateManager;

        public ActionPipelineManager(IActionProcessorRequirements requirements)
        {
            _log = Context.GetLogger();

            _requirements = requirements;
            
            _stateProcessors = new ConcurrentDictionary<string, IActorRef>();

            _stateManager = Context.ActorOf(ActionStateManager.Props(_requirements));

            Receive<RecoveryRequest>(request => OnRecoveryRequest(request));

            Receive<IActionRequest>(request => OnReceiveRequest(request),
                                   request => CanHandleRequest(request));
            
            Receive<IActionRequest>(request => OnUnhandledRequest(request));

            Receive<IActionResponse>(response => OnReceiveResponse(response));
        }

        protected override void PreStart() => _log.Debug("Action pipeline manager started");
        protected override void PostStop() => _log.Debug("Action pipeline manager stopped");

        public static Props Props(IActionProcessorRequirements requirements)
        {
            return Akka.Actor.Props.Create(() => new ActionPipelineManager(requirements));
        }

        public IDictionary<string, AllowedState> AllowedStates => _requirements.PipelineConfiguration.AllowedStates;

        private void OnRecoveryRequest(RecoveryRequest request)
        {
            var pendingRequests = _requirements.RequestPersistence.GetPendingRequests();

            foreach (IActionRequest pendingRequest in pendingRequests)
            {
                Self.Tell(pendingRequest.FlagForRecovery());
            }
        }

        private bool CanHandleRequest(IActionRequest request)
        {
            //TODO add error handling and logging

            var canHandle = _requirements.PipelineConfiguration.ActionTypes.ContainsKey(request.ActionName);

            if (!(request.CurrentState is null))
                canHandle = canHandle && _requirements.PipelineConfiguration.AllowedStates.ContainsKey(request.CurrentState.CurrentState);

            return canHandle;
        }

        private void OnReceiveRequest(IActionRequest request)
        {
            //TODO add error handling and logging

            if (!request.IsRecoveryRequest)
                _requirements.RequestPersistence.SavePendingRequest(request);

            if (request.CurrentState is null)
            {
                if (request is IStatePersistentRequest)
                {
                    _stateManager.Forward(RestoreStateRequest.New(request as IStatePersistentRequest));
                }
                else
                {
                    //TODO - Reject request
                    _log.Warning($"Could not process request for action '{request.ActionName}' ({request.RequestName}:{request.RequestId}). There is no current state defined for the request and state cannot be restored for a non-state persistent request");
                }
            }
            else
            {
                var currentStateName = request.CurrentState.CurrentState;
                if (!string.IsNullOrEmpty(currentStateName))
                {
                    if (_requirements.PipelineConfiguration.AllowedStates.ContainsKey(currentStateName))
                    {
                        var processor = _stateProcessors.GetOrAdd(currentStateName, (stateToAdd) =>
                        {
                            var state = _requirements.PipelineConfiguration.AllowedStates[stateToAdd];
                            return Context.ActorOf(ActionManager.Props(_requirements, state), $"{ActionManager.Name}_{currentStateName}");
                        });

                        processor.Forward(request);
                    }
                    else
                    {
                        Sender.Tell(UnhandledResponse.New(request, $"Request Rejected. The requested action '{request.ActionName}' is not valid for the current state '{currentStateName}'."));
                    }
                }
                else
                {
                    //TODO - Reject request
                    _log.Warning($"Could not process request for action '{request.ActionName}' ({request.RequestName}:{request.RequestId}). There is no current state defined for the request");
                }
            }
        }

        private void OnReceiveResponse(IActionResponse response)
        {
            //TODO add error handling and logging

            _requirements.RequestPersistence.DeletePendingRequest(response.Request.RequestId);

            var originalRequest = response.Request as IActionRequest;

            if (!(originalRequest is null) &&
                originalRequest is IStatePersistentRequest &&
                originalRequest.CurrentState != response.StateChange.NewState)
            {
                _stateManager.Tell(UpdateStateRequest.New(response));
            }
            else
            {
                if (response.Request is IContinuationRequest)
                {
                    var actionRequest = response.Request as IActionRequest;
                    var continuationRequest = response.Request as IContinuationRequest;
                    var nextRequestDescriptor = continuationRequest.NextRequest(response.StateChange.NewState);

                    if (!(nextRequestDescriptor is null))
                    {
                        var nextRequest = nextRequestDescriptor.ToRequest(actionRequest.PayloadType, response.StateChange.NewPayload, response.StateChange.NewState);
                        Self.Tell(nextRequest);
                    }
                }

                //Only publish responses to externally visible actions 
                var actionName = (response.Request as IAllowedAction).Name;
                var actionType = _requirements.PipelineConfiguration.ActionTypes[actionName];

                if (typeof(IExternalAction).IsAssignableFrom(actionType))
                    _requirements.ActionResponseObserver?.OnNext(response);
            }
        }

        private void OnUnhandledRequest(IActionRequest request)
        {
            Sender.Tell(UnhandledResponse.New(request, $"Request Rejected. The specified current state '{request.CurrentState.CurrentState}' is not a known state."));
        }
    }
}