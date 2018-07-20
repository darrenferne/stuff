using BWF.DataServices.Metadata.Fluent.Abstract;

namespace DataServiceDesigner.Domain
{
    public class SchemaBrowserMetadata : TypeMetadataProvider<DesignerSchemaBrowser>
    {
        public SchemaBrowserMetadata()
        {
            DisplayName("Schema Browser");

            TypeProperty(x => x.Connection)
                .DisplayName("Connection")
                .PositionInEditor(2)
                .Parameter(p => p
                    .DisplayProperty(x => x.Connection.Name)
                    .Query("DesignerConnections?$orderby=Name"))
                .DisplayFieldInEditorChoice("Name")
                .ValueFieldInEditorChoice("Id");

            StringProperty(x => x.SelectedSchema)
                .DisplayName("Selected Schema");

            CollectionProperty(x => x.AvailableObjects)
                .DisplayName("Available Objects");
        }
    }
}
