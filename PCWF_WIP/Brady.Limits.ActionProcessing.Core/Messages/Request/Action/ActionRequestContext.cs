using Akka.Actor;
using System.Threading.Tasks;

namespace Brady.Limits.ActionProcessing.Core
{
    internal static class ActionRequestContextExtensions
    {
        public static ActionRequestContext WithNewState(this ActionRequestContext context, IActionProcessingState newState)
        {
            return new ActionRequestContext(context.User, context.OriginatingActor, context.OriginatingRequest, newState, context.CompletionSource);
        }
    }

    public class ActionRequestContext: IActionRequestContext
    {
        public ActionRequestContext(
            IActionProcessorUser user,
            IActorRef originatingActor,
            IActionRequest originatingRequest)
            : this(user, originatingActor, originatingRequest, null,null)
        { }
        
        public ActionRequestContext(
            IActionProcessorUser user, 
            IActorRef originatingActor,
            IActionRequest originatingRequest,
            IActionProcessingState currentState,
            TaskCompletionSource<IResponse> completionSource)
        {
            User = user;
            OriginatingRequest = originatingRequest;
            OriginatingActor = originatingActor;
            ProcessingState = currentState;
            CompletionSource = completionSource;
        }

        public IActionProcessingState ProcessingState { get; }
        public IActionProcessorUser User { get; }
        public IActionRequest OriginatingRequest { get; }
        public IActorRef OriginatingActor { get; }
        public TaskCompletionSource<IResponse> CompletionSource { get; }
    }
}