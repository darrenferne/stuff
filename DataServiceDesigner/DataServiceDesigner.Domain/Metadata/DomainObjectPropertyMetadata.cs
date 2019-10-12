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

            TypeProperty(x => x.Object)
                .DisplayName("Object")
                .PositionInEditor(3)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjects?$orderby=Name'")
                .DisplayFieldInEditorChoice("Name")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode();

            StringProperty(x => x.ColumnName)
                .PositionInEditor(4)
                .DisplayName("Column Name");

            StringProperty(x => x.Name)
                .PositionInEditor(5);

            StringProperty(x => x.DisplayName)
                .DisplayName("Display Name")
                .PositionInEditor(6);

            BooleanProperty(x => x.IsPartOfKey)
                .DisplayName("Is Part Of Key?")
                .PositionInEditor(7);
                
            BooleanProperty(x => x.IncludeInDefaultView)
                .DisplayName("Include In Default View?")
                .PositionInEditor(8);

            ViewDefaults()
                .Property(x => x.Object.Schema.DataService.Name)
                .Property(x => x.Object.Schema.SchemaName)
                .Property(x => x.Object.Name)
                .Property(x => x.ColumnName)
                .Property(x => x.Name)
                .Property(x => x.DisplayName)
                .Property(x => x.IsPartOfKey)
                .Property(x => x.IncludeInDefaultView)
                .OrderBy(x => x.Name);
        }
    }
}
