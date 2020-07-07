using Akka.Actor;
using System.Threading.Tasks;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionRequestContext
    {
        IActionProcessingState ProcessingState { get; }
        IActionProcessorUser User { get; }
        IActionRequest OriginatingRequest { get; }
        IActorRef OriginatingActor { get; }
        TaskCompletionSource<IResponse> CompletionSource { get; }
    }
}