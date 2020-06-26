using Akka.Actor;
using Akka.Event;
using System;
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

            Receive<GetStateResponse>(response => OnStateRetrieved(response));
            Receive<UpdateStateResponse>(response => OnStateUpdated(response));

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

            if (!(request.Context.CurrentState is null))
                canHandle = canHandle && _requirements.PipelineConfiguration.AllowedStates.ContainsKey(request.Context.CurrentState.StateName);

            return canHandle;
        }

        private void SavePendingRequest(IActionRequest request)
        {
            if (!request.IsRecoveryRequest)
                _requirements.RequestPersistence.SavePendingRequest(request);
        }

        private void DeletePendingRequest(IActionRequest request)
        {
            if (!(request is null))
                _requirements.RequestPersistence.DeletePendingRequest(request.RequestId);
        }

        private bool UpdateState(IActionRequest request, IActionResponse response)
        {
            var newState = response.StateChange.NewState;

            if (!(request is null) &&
                request is IRequestWithState &&
                request.Context.CurrentState != newState)
            {
                _stateManager.Tell(UpdateStateRequest.New(request, response));
                return true;
            }
            else
                return false;
        }

        private void RestoreState(IActionRequest request)
        {
            if (request is IRequestWithState)
            {
                _stateManager.Tell(GetStateRequest.New(request as IRequestWithState), Self);
            }
            else
            {
                //TODO - Reject request
                _log.Warning($"Could not process request for action '{request.ActionName}' ({request.RequestName}:{request.RequestId}). There is no current state defined for the request and state cannot be restored for a non-state persistent request");
            }
        }

        private void ProcessAction(IActionRequest request)
        {
            var currentStateName = request.Context.CurrentState.StateName;
            if (!string.IsNullOrEmpty(currentStateName))
            {
                if (_requirements.PipelineConfiguration.AllowedStates.ContainsKey(currentStateName))
                {
                    var processor = _stateProcessors.GetOrAdd(currentStateName, (stateToAdd) =>
                    {
                        var state = _requirements.PipelineConfiguration.AllowedStates[stateToAdd];
                        return Context.ActorOf(ActionManager.Props(_requirements, state), $"{ActionManager.Name}_{currentStateName}");
                    });

                    processor.Tell(request);
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

        private void ProcessActionResponse(IActionResponse response)
        {
            if (!(response is null))
            {
                PublishResponse(response);

                if (!ProcessContinuation(response))
                    SetCompletion(response);
            }
        }

        private bool ProcessContinuation(IActionResponse response)
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
                    return true;
                }
            }

            return false;
        }

        private void SetCompletion(IActionResponse response)
        {
            if (response.Request is IActionRequest)
            {
                var request = response.Request as IActionRequest;
                var completionResponse = new ActionResponse(request.Context.OriginatingRequest, response.StateChange);

                if (!(request.Context.CompletionSource is null))
                {
                    request.Context.CompletionSource.TrySetResult(completionResponse);
                }

                PublishResponse(completionResponse);
            }
        }

        private void PublishResponse(IActionResponse response)
        {
            if (!(_requirements.ActionResponseObserver is null))
            {
                //Only publish responses to externally visible actions 
                var actionName = (response.Request as IAllowedAction).Name;
                var actionType = _requirements.PipelineConfiguration.ActionTypes[actionName];

                if (typeof(IExternalAction).IsAssignableFrom(actionType))
                    _requirements.ActionResponseObserver.OnNext(response);
            }
        }

        private void OnReceiveRequest(IActionRequest request)
        {
            //TODO add error handling and logging
            SavePendingRequest(request);

            if (request.Context.CurrentState is null)
            {
                RestoreState(request);
            }
            else
            {
                ProcessAction(request);
            }
        }

        private void OnReceiveResponse(IActionResponse response)
        {
            //TODO add error handling and logging

            var originalRequest = response.Request as IActionRequest;

            DeletePendingRequest(originalRequest);

            if (!UpdateState(originalRequest, response))
            {
                ProcessActionResponse(response);
            }
        }

        private void OnUnhandledRequest(IActionRequest request)
        {
            Sender.Tell(UnhandledResponse.New(request, $"Request Rejected. The specified current state '{request.Context.CurrentState.StateName}' is not a known state."));
        }

        private void OnStateUpdated(UpdateStateResponse response)
        {
            var originalRequestResponse = (response.Request as UpdateStateRequest).ForRequestResponse;

            ProcessActionResponse(originalRequestResponse);
        }

        private void OnStateRetrieved(GetStateResponse response)
        {
            var originalRequest = response.ForRequest as IRequestWithState;
            originalRequest.SetState(response.CurrentState);

            ProcessAction(originalRequest);
        }
    }
}