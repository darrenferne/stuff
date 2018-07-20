using BWF.DataServices.Metadata.Fluent.Abstract;

namespace SchemaBrowser.Domain
{
    public class DbObjectMetadata : TypeMetadataProvider<DbObject>
    {
        public DbObjectMetadata()
        {
            AutoUpdatesByDefault();
            
            DisplayName("Db Object");

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
                .DisplayName("Db Schema")
                .PositionInEditor(2)
                .Parameter(p => p
                    .Query("DbSchemas?$orderby=Name")
                    .DisplayProperty("Name"));

            StringProperty(x => x.Name)
                .DisplayName("Object Name")
                .PositionInEditor(3);
            
            EnumProperty(x => x.ObjectType)
                .DisplayName("Object Type")
                .PositionInEditor(4);

            //CollectionProperty(x => x.Properties)
            //    .DisplayName("Properties")
            //    .PositionInEditor(7);

            ExpandsForEdit();

            ViewDefaults()
                .Parameter(x => x.ConnectionId)
                .Parameter(x => x.SchemaName)
                .Property(x => x.SchemaName)
                .Property(x => x.Name)
                .OrderBy(x => x.SchemaName)
                .OrderBy(x => x.Name);
        }
    }
}
