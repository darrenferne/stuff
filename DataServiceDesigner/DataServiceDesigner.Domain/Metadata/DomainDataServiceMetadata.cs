using BWF.DataServices.Metadata.Fluent.Abstract;
using SchemaBrowser.Domain;
using SBD = SchemaBrowser.Domain;

namespace DataServiceDesigner.Domain
{
    public class DomainDataServiceMetadata : TypeMetadataProvider<DomainDataService>
    {
        public DomainDataServiceMetadata()
        {
            AutoUpdatesByDefault();
            SupportsEditMode();
            
            DisplayName("Data Service");

            IntegerProperty(x => x.Id)
                .IsId()
                .IsHiddenInEditor()
                .IsNotEditableInGrid();

            StringProperty(x => x.Name)
                .PositionInEditor(1);

            StringProperty(x => x.ConnectionString)
                .DisplayName("Connection String")
                .PositionInEditor(2)
                .CustomControl("cc-dataServiceConnection")
                .CustomControlHeight(30);

            TypeProperty(x => x.DbConnection)
                .FromDataService(Constants.DataServiceName)
                .IsHidden()
                .IsHiddenInEditor();            

            StringProperty(x => x.DefaultSchema)
                .DisplayName("Default Schema")
                .PositionInEditor(3);
            
            CollectionProperty(x => x.DomainObjects)
                .DisplayName("Domain Objects")
                .PositionInEditor(4)
                .CustomControl("cc-domainObjects")
                .CustomControlHeight(300);

            ViewDefaults()
                .Property(x => x.Name)
                .OrderBy(x => x.Name);
        }
    }
}
