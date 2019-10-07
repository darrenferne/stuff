using BWF.DataServices.Metadata.Fluent.Abstract;
using BWF.DataServices.Metadata.Fluent.Enums;

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

            TypeProperty(x => x.Connection)
                .IsHiddenInEditor()
                .IsNotEditableInGrid()
                .DisplayFieldInEditorChoice("Name")
                .ValueFieldInEditorChoice("Id");

            StringProperty(x => x.Name)
                .DisplayName("Schema Name")
                .Parameter(p => p
                    .Query("DbSchemas?$orderby=Name")
                    .DisplayProperty("Name")
                    .AllowNullOrEmpty()
                    .AvailableOperators(Operator.Equals)); 

            ViewDefaults()
                .Parameter(x => x.Connection.Name)
                .Property(x => x.Name)
                .OrderBy(x => x.Name);
        }
    }
}
