﻿using BWF.DataServices.Metadata.Fluent.Abstract;

namespace DataServiceDesigner.Domain
{
    public class MetadataBundle : TypeMetadataBundle
    {
        public MetadataBundle()
            : base("dataservicedesigner",
                new DataServiceConnectionMetadata(), 
                new DomainDataServiceMetadata(),
                new DomainObjectMetadata(),
                new DomainObjectPropertyMetadata()
                  //,new SchemaBrowserMetadata()
                  )
        { }
    }
}