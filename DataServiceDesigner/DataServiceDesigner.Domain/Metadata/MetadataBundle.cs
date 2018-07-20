using BWF.DataServices.Metadata.Fluent.Abstract;

namespace DataServiceDesigner.Domain
{
    public class MetadataBundle : TypeMetadataBundle
    {
        public MetadataBundle()
            : base("dataservicedesigner",
                  new DataServiceMetadata(),
                  //new ConnectionMetadata(),
                  new DomainObjectMetadata(),
                  new DomainObjectPropertyMetadata()
                  //,new SchemaBrowserMetadata()
                  )
        { }
    }
}
