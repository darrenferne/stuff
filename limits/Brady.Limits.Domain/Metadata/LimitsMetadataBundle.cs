using BWF.DataServices.Metadata.Fluent.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brady.Limits.Domain.Metadata
{
    public class LimitsMetadataBundle : TypeMetadataBundle
    {
        public LimitsMetadataBundle()
            : base(Constants.DataServiceName, 
                  new SelectionMetadata(), 
                  new WorkflowMetadata(),
                  new WorkflowLevelMetadata())
        { }
    }
}
