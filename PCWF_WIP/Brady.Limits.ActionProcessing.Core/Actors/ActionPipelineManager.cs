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
            Receive<UnhandledResponse>(response => OnUnhandledResponse(response));

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
                if (pendingRequest is IRequestWithContext)
                {
                    var contextRequest = pendingRequest as IRequestWithContext;
                    var user = new ActionProcessorUser(_requirements.Authorisation, _requirements.Authorisation?.SystemUserName);
                    var context = new ActionRequestContext(user, Self, pendingRequest as IActionRequest, null, null);

                    contextRequest.InitialiseContext(context);
                }

                Self.Tell(pendingRequest.FlagForRecovery());
            }
        }

        private bool CanHandleRequest(IActionRequest request)
        {
            //TODO add error handling and logging

            var canHandle = _requirements.PipelineConfiguration.ActionTypes.ContainsKey(request.ActionName);
            if (canHandle)
            {
                if (!typeof(IStateCheckAction).IsAssignableFrom(request.RequestType) && 
                    !(request.Context.ProcessingState is null))
                { 
                    canHandle = canHandle && _requirements.PipelineConfiguration.AllowedStates.ContainsKey(request.Context.ProcessingState.CurrentState);
                }
            }

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
            var stateChange = response.Result as IActionProcessingStateChange;
            var newState = stateChange.NewState;

            if (!(request is null) &&
                request is IRequestWithState &&
                request.Context.ProcessingState != newState)
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
                SetRejected(request, $"Could not process request for action '{request.ActionName}' ({request.RequestName}:{request.RequestId}). There is no current state defined for the request and state cannot be restored for a non-state persistent request");
            }
        }

        private void ProcessAction(IActionRequest request)
        {
            var currentStateName = request.Context.ProcessingState.CurrentState;
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
                    SetRejected(request, $"Request Rejected. The requested action '{request.ActionName}' is not valid for the current state '{currentStateName}'.");
                }
            }
            else
            {
                SetRejected(request, $"Could not process request for action '{request.ActionName}' ({request.RequestName}:{request.RequestId}). There is no current state defined for the request");
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
                var stateChange = response.Result as IActionProcessingStateChange;
                var nextRequestDescriptor = continuationRequest.NextRequest(stateChange.NewState);

                if (!(nextRequestDescriptor is null))
                {
                    var nextRequest = nextRequestDescriptor.ToRequest(actionRequest.PayloadType, stateChange.NewPayload);
                    if (nextRequest is IRequestWithContext && response.Request is IActionRequest)
                        (nextRequest as IRequestWithContext).InitialiseContext((response.Request as IActionRequest).Context);

                    Self.Tell(nextRequest);
                    return true;
                }
            }

            return false;
        }

        private void SetRejected(IActionRequest request, string message)
        {
            var response = new UnhandledResponse(request.Context.OriginatingRequest, message);
            SetRejected(request.Context, response);
        }

        private void SetRejected(IActionRequestContext context, UnhandledResponse response)
        {
            _log.Warning(response.Message);
            if (!(context.CompletionSource is null))
            {
                context.CompletionSource.TrySetResult(response);
            }
        }
        
        private void SetCompletion(IActionResponse response)
        {
            if (response.Request is IActionRequest)
            {
                var request = response.Request as IActionRequest;
                var stateChange = response.Result as IActionProcessingStateChange;
                var completionResponse = new ActionResponse(request.Context.OriginatingRequest, stateChange);

                if (!(request.Context.CompletionSource is null))
                {
                    request.Context.CompletionSource.TrySetResult(completionResponse);
                }

                PublishResponse(completionResponse);
            }
        }

        private void PublishResponse(IActionResponse response)
        {
            if (!(response is null) && !(_requirements.ActionResponseObserver is null))
            {
                var actionRequest = response.Request as IActionRequest;
                if (!(actionRequest is null))
                {
                    var actionName = actionRequest.ActionName;
                    var actionType = _requirements.PipelineConfiguration.ActionTypes[actionName];
                    
                    //Only publish responses to externally visible actions 
                    if (typeof(IExternalAction).IsAssignableFrom(actionType))
                        _requirements.ActionResponseObserver.OnNext(response);
                }
            }
        }

        private void OnReceiveRequest(IActionRequest request)
        {
            //TODO add error handling and logging
            SavePendingRequest(request);

            if (request.Context.ProcessingState is null)
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
            //TODO - This doesn't report invalid actions. It just assumes it's an invalid state 
            SetRejected(request, $"Request Rejected. The specified current state '{request.Context.ProcessingState.CurrentState}' is not a known state.");
        }

        private void OnUnhandledResponse(UnhandledResponse response)
        {
            var context = (response.Request as IActionRequest).Context;

            SetRejected(context, response);
        }
        
        private void OnStateUpdated(UpdateStateResponse response)
        {
            var updateStateRequest = response.Request as UpdateStateRequest;
            var originalRequest = updateStateRequest.ForRequest as IRequestWithState;
            var originalRequestResponse = updateStateRequest.ForRequestResponse;
            var stateChange = originalRequestResponse.Result as IActionProcessingStateChange;
            var newState = stateChange.NewState;

            originalRequest.SetState(newState);
            
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