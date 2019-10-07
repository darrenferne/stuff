using BWF.DataServices.Metadata.Fluent.Abstract;
using BWF.DataServices.Metadata.Fluent.Enums;

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

            TypeProperty(x => x.Connection)
                .IsHiddenInEditor()
                .IsNotEditableInGrid()
                .DisplayFieldInEditorChoice("Name")
                .ValueFieldInEditorChoice("Id");

            StringProperty(x => x.SchemaName)
                .PositionInEditor(1)
                .DisplayName("Db Schema")
                .Parameter(p => p
                    .Query("DbSchemas?$orderby=Name")
                    .DisplayProperty("Name")
                    .AllowNullOrEmpty()
                    .AvailableOperators(Operator.Equals));

            StringProperty(x => x.ObjectName)
                .PositionInEditor(1)
                .DisplayName("Db Object")
                .Parameter(p => p
                    .Query("DbObjects?$filter=&orderby=Name")
                    .DisplayProperty("Name")
                    .AllowNullOrEmpty()
                    .AvailableOperators(Operator.Equals));

            StringProperty(x => x.Name)
                .PositionInEditor(2)
                .DisplayName("Property Name");

            ViewDefaults()
                .Parameter(x => x.Connection.Name)
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
