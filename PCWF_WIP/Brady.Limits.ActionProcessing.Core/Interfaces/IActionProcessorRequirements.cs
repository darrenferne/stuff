namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessorRequirements
    {
        IActionPipelineConfiguration PipelineConfiguration { get; }

        IActionProcessingRequestPersistence RequestPersistence { get; }

        IActionProcessingStatePersistence StatePersistence { get; }

        IActionResponseObserver ActionResponseObserver { get; }
    }
}
