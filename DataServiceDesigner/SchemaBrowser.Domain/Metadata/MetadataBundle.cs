using BWF.DataServices.Metadata.Fluent.Abstract;

namespace SchemaBrowser.Domain
{
    public class MetadataBundle : TypeMetadataBundle
    {
        public MetadataBundle()
            : base(Constants.DataServiceName,
                  new DbConnectionMetadata(),
                  new DbSchemaMetadata(),
                  new DbObjectMetadata(),
                  new DbObjectPropertyMetadata(),
                  new DbObjectPrimaryKeyMetadata(),
                  new DbObjectForeignKeyMetadata())
        { }
    }
}
