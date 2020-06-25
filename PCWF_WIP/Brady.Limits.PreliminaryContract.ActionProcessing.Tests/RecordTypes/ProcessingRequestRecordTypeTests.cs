using Brady.Limits.ActionProcessing.DataService;
using Brady.Limits.PreliminaryContract.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Brady.Limits.PreliminaryContract.ActionProcessing.Tests
{
    [TestClass]
    public class ProcessingRequestRecordTypeTests : RecordTypeTestsBase<ProcessingRequest>
    {
        public override (ProcessingRequest, IList<string>) ConstructInvalidRecord()
        {
            return (new ProcessingRequest
            {
                RequestId = new string('X', 40),
                RequestType = new string('X', 70)
            }, new List<string> {
                "'Request Id' must be between 1 and 36 characters. You entered 40 characters.",
                "'Request Type' must be between 1 and 64 characters. You entered 70 characters."
            });
        }

        public override ProcessingRequest ConstructValidRecord()
        {
            return new ProcessingRequest
            {
                RequestId = Guid.NewGuid().ToString("N"),
                RequestType = "TestRequestType",
                RequestBody = "Test Request Body"
            };
        }
        public override ProcessingRequest UpdateRecordWithValidDetails(ProcessingRequest record)
        {
            record.RequestId = Guid.NewGuid().ToString("N");
            return record;
        }

        public override (ProcessingRequest, IList<string>) UpdateRecordWithInvalidDetails(ProcessingRequest record)
        {
            record.RequestId = new string('X', 40);
            record.RequestType = new string('X', 70);

            return (record, new List<string> {
                "'Request Id' must be between 1 and 36 characters. You entered 40 characters.",
                "'Request Type' must be between 1 and 64 characters. You entered 70 characters."
            });
        }
    }
}
