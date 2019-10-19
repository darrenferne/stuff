using BWF.DataServices.Metadata.Fluent.Abstract;
using BWF.DataServices.Metadata.Fluent.Enums;

namespace SchemaBrowser.Domain
{
    public class DbObjectForeignKeyMetadata : TypeMetadataProvider<DbObjectForeignKey>
    {
        public DbObjectForeignKeyMetadata()
        {
            AutoUpdatesByDefault();
            
            DisplayName("Db Object Foreign Key");

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
            
            EnumProperty(x => x.ConstraintName)
                .DisplayName("Constraint Name")
                .PositionInEditor(4);

            StringProperty(x => x.ColumnSummary)
                .DisplayName("Column Summary")
                .PositionInEditor(5)
                .DisableInCreateMode(true)
                .DisableInEditMode(true);

            StringProperty(x => x.ReferencedIndexSummary)
                .DisplayName("Referenced Index Summary")
                .PositionInEditor(5)
                .DisableInCreateMode(true)
                .DisableInEditMode(true);

            ExpandsForEdit();

            ViewDefaults()
                .Parameter(x => x.Connection.Name)
                .Parameter(x => x.SchemaName)
                .Property(x => x.SchemaName)
                .Property(x => x.TableName)
                .Property(x => x.ConstraintName)
                .Property(x => x.ColumnSummary)
                .Property(x => x.ReferencedIndexSummary)
                .OrderBy(x => x.SchemaName)
                .OrderBy(x => x.TableName);
        }
    }
}
