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
                .DisplayName("Id")
                .IsId()
                .IsHiddenInEditor()
                .IsNotEditableInGrid();

            TypeProperty(x => x.Object)
                .DisplayName("Object")
                .PositionInEditor(3)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjects?$orderby=ObjectName'")
                .DisplayFieldInEditorChoice("Name")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode();

            StringProperty(x => x.ColumnName)
                .PositionInEditor(4)
                .DisplayName("Column Name");

            StringProperty(x => x.PropertyName)
                .DisplayName("Property Name")
                .PositionInEditor(5);

            StringProperty(x => x.DisplayName)
                .DisplayName("Display Name")
                .PositionInEditor(6);

            EnumProperty(x => x.PropertyType)
                .DisplayName("Property Type")
                .PositionInEditor(7);

            StringProperty(x => x.Length)
                .DisplayName("Length")
                .PositionInEditor(8);

            StringProperty(x => x.IsNullable)
                .DisplayName("Nullable?")
                .PositionInEditor(9);

            BooleanProperty(x => x.IsPartOfKey)
                .DisplayName("Is Part Of Key?")
                .PositionInEditor(10);
                
            BooleanProperty(x => x.IncludeInDefaultView)
                .DisplayName("Include In Default View?")
                .PositionInEditor(11);

            ViewDefaults()
                .Property(x => x.Object.Schema.DataService.Name)
                .Property(x => x.Object.Schema.SchemaName)
                .Property(x => x.Object.ObjectName)
                .Property(x => x.ColumnName)
                .Property(x => x.PropertyName)
                .Property(x => x.DisplayName)
                .Property(x => x.PropertyType)
                .Property(x => x.Length)
                .Property(x => x.IsNullable)
                .Property(x => x.IsPartOfKey)
                .Property(x => x.IncludeInDefaultView)
                .OrderBy(x => x.PropertyName);
        }
    }
}
