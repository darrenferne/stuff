using BWF.DataServices.Metadata.Fluent.Abstract;

namespace DataServiceDesigner.Domain
{
    public class DomainObjectPropertyMetadata : TypeMetadataProvider<DomainObjectProperty>
    {
        public DomainObjectPropertyMetadata()
        {
            AutoUpdatesByDefault();
            SupportsEditMode();

            DisplayName("Domain Object Property");
            PluralisedDisplayName("Domain Object Properties");

            IntegerProperty(x => x.Id)
                .IsId()
                .IsHiddenInEditor()
                .IsNotEditableInGrid();

            TypeProperty(p => p.DataService)
                .DisplayName("Data Service")
                .PositionInEditor(1)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainDataServices?$orderby=Name'")
                .DisplayFieldInEditorChoice(p => p.Name)
                .ValueFieldInEditorChoice(p => p.Id)
                .IsMandatoryInEditMode();

            TypeProperty(x => x.Object)
                .DisplayName("Domain Object")
                .PositionInEditor(1)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjects?$orderby=Name'")
                .DisplayFieldInEditorChoice("Name")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode();

            StringProperty(x => x.DbName)
                .PositionInEditor(2)
                .DisplayName("Db Name");

            StringProperty(x => x.Name)
                .PositionInEditor(3);

            StringProperty(x => x.DisplayName)
                .DisplayName("Display Name")
                .PositionInEditor(4);

            BooleanProperty(x => x.IsPartOfKey)
                .DisplayName("Is Part Of Key?")
                .PositionInEditor(5);
                
            BooleanProperty(x => x.IncludeInDefaultView)
                .DisplayName("Include In Default View?")
                .PositionInEditor(6);

            ViewDefaults()
                .Property(x => x.DataService.Name)
                .Property(x => x.Object.Name)
                .Property(x => x.DbName)
                .Property(x => x.Name)
                .Property(x => x.DisplayName)
                .Property(x => x.IsPartOfKey)
                .Property(x => x.IncludeInDefaultView)
                .OrderBy(x => x.Name);
        }
    }
}
