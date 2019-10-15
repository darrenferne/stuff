using BWF.DataServices.Metadata.Fluent.Abstract;
using BWF.DataServices.Metadata.Fluent.Enums;

namespace SchemaBrowser.Domain
{
    public class DbObjectPrimaryKeyMetadata : TypeMetadataProvider<DbObjectPrimaryKey>
    {
        public DbObjectPrimaryKeyMetadata()
        {
            AutoUpdatesByDefault();
            
            DisplayName("Db Object Primary Key");

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

            StringProperty(x => x.TableName)
                .DisplayName("Table Name")
                .PositionInEditor(3);
            
            EnumProperty(x => x.IndexName)
                .DisplayName("Index Name")
                .PositionInEditor(4);

            StringProperty(x => x.ColumnSummary)
                .DisplayName("Column Summary")
                .PositionInEditor(5)
                .DisableInCreateMode(true)
                .DisableInEditMode(true);

            ExpandsForEdit();

            ViewDefaults()
                .Parameter(x => x.Connection.Name)
                .Parameter(x => x.SchemaName)
                .Property(x => x.SchemaName)
                .Property(x => x.TableName)
                .Property(x => x.IndexName)
                .Property(x => x.ColumnSummary)
                .OrderBy(x => x.SchemaName)
                .OrderBy(x => x.TableName);
        }
    }
}
