using System;
using Brady.Limits.ActionProcessing.Core;
using Brady.Limits.PreliminaryContract.Domain.Models;

namespace Brady.Limits.PreliminaryContract.ActionProcessing
{
    public interface IContractProcessingPayload : IActionRequestPayload
    {
        Contract Contract { get; }
        Contract PreviousVersion { get; }
    }
}