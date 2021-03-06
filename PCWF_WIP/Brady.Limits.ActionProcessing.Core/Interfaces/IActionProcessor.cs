﻿using System.Threading.Tasks;

namespace Brady.Limits.ActionProcessing.Core
{
    public interface IActionProcessor
    {
        ActionProcessorState State { get; }
        void Start(bool withRecovery = true);
        void Stop();
        
        Task<IResponse> ProcessAction<TRequest>(TRequest request, string userName = null)
            where TRequest : class, IRequestWithContext;
    }
}