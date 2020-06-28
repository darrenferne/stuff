using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing.Tests
{
    public class TestProcessorRequirements : IPreliminaryContractActionProcessorRequirements
    {
        public TestProcessorRequirements(
            IActionPipelineConfiguration pipelineConfiguration, 
            IActionProcessingRequestPersistence requestPersistence,
            IActionProcessingStatePersistence statePersistence,
            IActionProcessorAuthorisation authorisation = null,
            IActionResponseObserver actionResponseObserver = null)
        {
            Authorisation = authorisation;
            PipelineConfiguration = pipelineConfiguration;
            RequestPersistence = requestPersistence;
            StatePersistence = statePersistence;
            ActionResponseObserver = actionResponseObserver;
        }

        public IActionProcessorAuthorisation Authorisation { get; }
        public IActionPipelineConfiguration PipelineConfiguration { get; }
        public IActionProcessingRequestPersistence RequestPersistence { get; }
        public IActionProcessingStatePersistence StatePersistence { get; }
        public IActionResponseObserver ActionResponseObserver { get; }
    }
}
