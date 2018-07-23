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

            TypeProperty(x => x.DomainObject)
                .DisplayName("Domain Object")
                .PositionInEditor(1)
                .Parameter(p => p
                    .DisplayProperty(x => x.DomainObject.Name)
                    .Query("DesignerDomainObjects?$orderby=Name"))
                .DisplayFieldInEditorChoice("Name")
                .ValueFieldInEditorChoice("Id");

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
                .Property(x => x.DbName)
                .Property(x => x.Name)
                .Property(x => x.DisplayName)
                .Property(x => x.IsPartOfKey)
                .Property(x => x.IncludeInDefaultView)
                .OrderBy(x => x.Name);
        }
    }
}
