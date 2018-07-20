using BWF.DataServices.Metadata.Fluent.Abstract;

namespace SchemaBrowser.Domain
{
    public class DbObjectPropertyMetadata : TypeMetadataProvider<DbObjectProperty>
    {
        public DbObjectPropertyMetadata()
        {
            AutoUpdatesByDefault();

            DisplayName("Db Object Property");
            PluralisedDisplayName("Db Object Properties");

            IntegerProperty(x => x.Id)
                .IsId()
                .IsHiddenInEditor()
                .IsNotEditableInGrid();

            IntegerProperty(x => x.ConnectionId)
                .IsHiddenInEditor()
                .IsNotEditableInGrid()
                .Parameter(p => p
                    .Query("DbConnections?$orderby=Name")
                    .DisplayProperty("Name"));

            StringProperty(x => x.SchemaName)
                .PositionInEditor(1)
                .DisplayName("Db Schema")
                .Parameter(p => p
                    .Query("DbSchemas?$orderby=Name")
                    .DisplayProperty("Name"));

            StringProperty(x => x.ObjectName)
                .PositionInEditor(1)
                .DisplayName("Db Object")
                .Parameter(p => p
                    .Query("DbObjects?$filter=&orderby=Name")
                    .DisplayProperty("Name"));

            StringProperty(x => x.Name)
                .PositionInEditor(2)
                .DisplayName("Property Name");

            ViewDefaults()
                .Parameter(x => x.ConnectionId)
                .Parameter(x => x.SchemaName)
                .Parameter(x => x.ObjectName)
                .Property(x => x.SchemaName)
                .Property(x => x.ObjectName)
                .Property(x => x.Name)
                .OrderBy(x => x.SchemaName)
                .OrderBy(x => x.ObjectName)
                .OrderBy(x => x.Name);
        }
    }
}
