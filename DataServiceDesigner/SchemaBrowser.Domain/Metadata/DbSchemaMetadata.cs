using BWF.DataServices.Metadata.Fluent.Abstract;

namespace SchemaBrowser.Domain
{
    public class DbSchemaMetadata : TypeMetadataProvider<DbSchema>
    {
        public DbSchemaMetadata()
        {
            AutoUpdatesByDefault();
            
            DisplayName("Db Schema");

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

            StringProperty(x => x.Name)
                .DisplayName("Schema Name")
                .Parameter(p => p
                    .Query("DbSchemas?$orderby=Name")
                    .DisplayProperty("Name")); 

            ViewDefaults()
                .Parameter(x => x.ConnectionId)
                .Property(x => x.Name)
                .OrderBy(x => x.Name);
        }
    }
}
