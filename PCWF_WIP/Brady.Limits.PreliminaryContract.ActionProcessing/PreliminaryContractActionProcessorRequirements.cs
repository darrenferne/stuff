using Brady.Limits.ActionProcessing.Core;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    class PreliminaryContractActionProcessorRequirements : IPreliminaryContractActionProcessorRequirements
    {
        public PreliminaryContractActionProcessorRequirements(
            IActionProcessorAuthorisation authorisation,
            IActionPipelineConfiguration pipelineConfiguration, 
            IActionProcessingRequestPersistence requestPersistence,
            IActionProcessingStatePersistence statePersistence,
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
