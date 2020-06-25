using Brady.Limits.ActionProcessing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    class PreliminaryContractActionProcessorRequirements : IPreliminaryContractActionProcessorRequirements
    {
        public PreliminaryContractActionProcessorRequirements(
            IActionPipelineConfiguration pipelineConfiguration, 
            IActionProcessingRequestPersistence requestPersistence,
            IActionProcessingStatePersistence statePersistence,
            IActionResponseObserver actionResponseObserver = null)
        {
            PipelineConfiguration = pipelineConfiguration;
            RequestPersistence = requestPersistence;
            StatePersistence = statePersistence;
            ActionResponseObserver = actionResponseObserver;
        }

        public IActionPipelineConfiguration PipelineConfiguration { get; }
        public IActionProcessingRequestPersistence RequestPersistence { get; }
        public IActionProcessingStatePersistence StatePersistence { get; }
        public IActionResponseObserver ActionResponseObserver { get; }
    }
}
