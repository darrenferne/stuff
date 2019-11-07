using BWF.DataServices.Metadata.Fluent.Abstract;

namespace DataServiceDesigner.Domain
{
    public class MetadataBundle : TypeMetadataBundle
    {
        public MetadataBundle()
            : base("dataservicedesigner",
                new DataServiceSolutionMetadata(),
                new DataServiceConnectionMetadata(),
                new DomainDataServiceMetadata(),
                new DomainSchemaMetadata(),
                new DomainObjectMetadata(),
                new DomainObjectPropertyMetadata(),
                new DomainObjectReferenceMetadata(),
                new DomainObjectReferencePropertyMetadata()
            )
        { }
    }
}
