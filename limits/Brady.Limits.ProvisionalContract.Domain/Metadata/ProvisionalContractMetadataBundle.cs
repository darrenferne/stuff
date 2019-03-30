using BWF.DataServices.Metadata.Fluent.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brady.Limits.ProvisionalContract.Domain.Metadata
{
    public class ProvisionalContractMetadataBundle : TypeMetadataBundle
    {
        public ProvisionalContractMetadataBundle()
            : base(Constants.DataServiceName, 
                  new ProvisionalContractMetadata())
        { }
    }
}
