using BWF.DataServices.Metadata.Fluent.Abstract;

namespace DataServiceDesigner.Domain
{
    public class DataServiceConnectionMetadata : TypeMetadataProvider<DataServiceConnection>
    {
        public DataServiceConnectionMetadata()
        {
            AutoUpdatesByDefault();
            SupportsEditMode();

            DisplayName("Connection");
            
            IntegerProperty(x => x.Id)
                .IsId()
                .IsHiddenInEditor()
                .IsNotEditableInGrid();

            StringProperty(x => x.Name)
                .PositionInEditor(1);

            EnumProperty(x => x.DatabaseType)
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
                .CustomControl("cc-connectionString")
                .CustomControlHeight(30);

            ViewDefaults()
                .Property(x => x.Name)
                .Property(x => x.DatabaseType)
                .Property(x => x.ConnectionString)
                .OrderBy(x => x.Name);
        }
    }
}
