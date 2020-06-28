using Akka.Actor;
using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionRequestContext
    {
        IActionProcessingState CurrentState { get; }
        IActionProcessorUser User { get; }
        IActionRequest OriginatingRequest { get; }
        IActorRef OriginatingActor { get; }
        TaskCompletionSource<IResponse> CompletionSource { get; }
    }
}