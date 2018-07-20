using BWF.DataServices.Metadata.Fluent.Abstract;
using SBD = SchemaBrowser.Domain;

namespace DataServiceDesigner.Domain
{
    public class DataServiceMetadata : TypeMetadataProvider<DesignerDataService>
    {
        public DataServiceMetadata()
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

            TypeProperty(x => x.Connection)
                .FromDataService(SBD.Constants.DataServiceName)
                .JoinedOn("ConnectionId", "Id")
                .DisplayName("Connection")
                .PositionInEditor(2)
                .DisplayFieldInEditorChoice("Name")
                .ValueFieldInEditorChoice("Id");

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
