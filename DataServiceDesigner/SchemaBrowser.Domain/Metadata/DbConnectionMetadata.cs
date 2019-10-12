using BWF.DataServices.Metadata.Fluent.Abstract;
using BWF.DataServices.Metadata.Fluent.Enums;

namespace SchemaBrowser.Domain
{
    public class DbConnectionMetadata : TypeMetadataProvider<DbConnection>
    {
        public DbConnectionMetadata()
        {
            AutoUpdatesByDefault();
            
            DisplayName("Db Connection");
            
            IntegerProperty(x => x.Id)
                .IsId()
                .IsHiddenInEditor()
                .IsNotEditableInGrid();

            IntegerProperty(x => x.ExternalId)
                .IsHiddenInEditor();

            StringProperty(x => x.Name)
                .Parameter(p => p
                    .Query("DbConnections?$orderby=Name")
                    .DisplayProperty(o => o.Name)
                    .AllowNullOrEmpty()
                    .AvailableOperators(Operator.Equals))
                .PositionInEditor(1);

            EnumProperty(x => x.DatabaseType)
                .DisplayName("Database Type")
                .PositionInEditor(2);

            StringProperty(x => x.DataSource)
                .DisplayName("Data Source")
                .PositionInEditor(3);

            StringProperty(x => x.InitialCatalog)
                .DisplayName("Initial Catalog")
                .PositionInEditor(4)
                .CustomControl("cc-initialCatalog")
                .CustomControlHeight(30);

            BooleanProperty(x => x.UseIntegratedSecurity)
                .DisplayName("Use Integrated Security")
                .PositionInEditor(5);

            StringProperty(x => x.Username)
                .DisplayName("User Name")
                .PositionInEditor(6)
                .CustomControl("cc-username")
                .CustomControlHeight(30); ;

            StringProperty(x => x.Password)
                .PositionInEditor(7)
                .CustomControl("cc-password")
                .CustomControlHeight(30);
            
            StringProperty(x => x.ConnectionString)
                .DisplayName("Connection String")
                .PositionInEditor(8)
                .IsNotEditableInGrid()
                .DisableInEditMode()
                .DisableInCreateMode()
                .CustomControl("cc-connectionString")
                .CustomControlHeight(60);

            EnumProperty(x => x.Status)
                .IsHiddenInEditor();

            StringProperty(x => x.StatusMessage)
                .DisplayName("Status Message")
                .IsHiddenInEditor();

            ViewDefaults()
                .Property(x => x.Name)
                .Property(x => x.DatabaseType)
                .Property(x => x.ConnectionString)
                .Property(x => x.Status)
                .Property(x => x.StatusMessage)
                .OrderBy(x => x.Name);
        }
    }
}
