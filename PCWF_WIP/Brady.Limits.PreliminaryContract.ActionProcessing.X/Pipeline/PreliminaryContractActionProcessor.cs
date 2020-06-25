using Akka.Actor;
using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class PreliminaryContractActionProcessor : ActionProcessor
    {
        public PreliminaryContractActionProcessor(IPreliminaryContractActionProcessorRequirements requirements, ActorSystem actorSystem)
            : base(requirements, actorSystem)
        { }
    }
}
