using Akka.Actor;
using Brady.Limits.ActionProcessing.Core;
using System;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public class PreliminaryContractActionProcessor : ActionProcessor
    {
        public PreliminaryContractActionProcessor(IPreliminaryContractActionProcessorRequirements requirements, ActorSystem actorSystem)
            : base(requirements, actorSystem)
        { }
    }
}
