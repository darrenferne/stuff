using BWF.DataServices.Metadata.Fluent.Abstract;
using BWF.DataServices.Metadata.Fluent.Enums;

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

            TypeProperty(x => x.Connection)
                .IsHiddenInEditor()
                .IsNotEditableInGrid()
                .DisplayFieldInEditorChoice("Name")
                .ValueFieldInEditorChoice("Id");

            StringProperty(x => x.SchemaName)
                .DisplayName("Db Schema")
                .PositionInEditor(2)
                .Parameter(p => p
                    .Query("DbSchemas?$orderby=Name")
                    .DisplayProperty("Name")
                    .AllowNullOrEmpty()
                    .AvailableOperators(Operator.Equals));

            StringProperty(x => x.Name)
                .DisplayName("Object Name")
                .PositionInEditor(3);
            
            EnumProperty(x => x.ObjectType)
                .DisplayName("Object Type")
                .PositionInEditor(4);
            
            ExpandsForEdit();

            ViewDefaults()
                .Parameter(x => x.Connection.Name)
                .Parameter(x => x.SchemaName)
                .Property(x => x.SchemaName)
                .Property(x => x.Name)
                .OrderBy(x => x.SchemaName)
                .OrderBy(x => x.Name);
        }
    }
}
