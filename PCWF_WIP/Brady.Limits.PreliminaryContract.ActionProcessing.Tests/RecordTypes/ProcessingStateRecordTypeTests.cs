using Brady.Limits.ActionProcessing.DataService;
using Brady.Limits.PreliminaryContract.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Brady.Limits.PreliminaryContract.ActionProcessing.Tests
{
    [TestClass]
    public class ProcessingStateRecordTypeTests : RecordTypeTestsBase<ProcessingState>
    {
        public override (ProcessingState, IList<string>) ConstructInvalidRecord()
        {
            return (new ProcessingState
            {
                ExternalId = new string('X', 70),
                StateType = new string('X', 70)
            }, new List<string> {
                "'External Id' must be between 1 and 64 characters. You entered 70 characters.",
                "'State Type' must be between 1 and 64 characters. You entered 70 characters."
            });
        }

        public override ProcessingState ConstructValidRecord()
        {
            return new ProcessingState
            {
                ExternalId = Guid.NewGuid().ToString(),
                StateType = "TestStateType",
                StateBody = "Test State Body"
            };
        }
        public override ProcessingState UpdateRecordWithValidDetails(ProcessingState record)
        {
            record.StateBody = "Updates State Body";
            return record;
        }

        public override (ProcessingState, IList<string>) UpdateRecordWithInvalidDetails(ProcessingState record)
        {
            record.StateType = new string('X', 70);

            return (record, new List<string> {
                "'State Type' must be between 1 and 64 characters. You entered 70 characters."
            });
        }
    }
}
