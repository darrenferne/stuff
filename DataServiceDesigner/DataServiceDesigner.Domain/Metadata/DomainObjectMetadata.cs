using BWF.DataServices.Metadata.Fluent.Abstract;

namespace DataServiceDesigner.Domain
{
    public class DomainObjectMetadata : TypeMetadataProvider<DomainObject>
    {
        public DomainObjectMetadata()
        {
            AutoUpdatesByDefault();
            SupportsEditMode();

            DisplayName("Domain Object");

            IntegerProperty(x => x.Id)
                .IsId()
                .IsHiddenInEditor()
                .IsNotEditableInGrid();

            TypeProperty(x => x.DataService)
                .DisplayName("Data Service")
                .PositionInEditor(1)
                .Parameter(p => p
                    .DisplayProperty(x => x.DataService.Name)
                    .Query("DesignerDataServices?$orderby=Name"))
                .DisplayFieldInEditorChoice("Name")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode();

            StringProperty(x => x.DbSchema)
                .DisplayName("Db Schema")
                .PositionInEditor(2)
                .DisplayFieldInEditorChoice(x => x.Name)
                .ValueFieldInEditorChoice(x => x.Name)
                .RefreshChoiceOnChangeTo(x => x.DataService)
                .PopulateChoiceQuery(@"this.dataService + '/ext/availableschemas/' + this.selectedDataService().Id");

            StringProperty(x => x.DbObject)
                .DisplayName("Db Object")
                .PositionInEditor(3)
                .DisplayFieldInEditorChoice(x => x.Name)
                .ValueFieldInEditorChoice(x => x.Name)
                .RefreshChoiceOnChangeTo(x => x.DataService, x => x.DbSchema)
                .PopulateChoiceQuery(@"this.dataService + '/ext/schemaobjects/' + this.selectedDataService().Id + '/' + this.selectedDbSchema().Name");

            StringProperty(x => x.Name)
                .PositionInEditor(4);

            StringProperty(x => x.DisplayName)
                 .DisplayName("Display Name")
                 .PositionInEditor(5);

            StringProperty(x => x.PluralisedDisplayName)
                 .DisplayName("Pluralised Display Name")
                 .PositionInEditor(6);
            
            CollectionProperty(x => x.ObjectProperties)
                .DisplayName("Object Properties")
                .PositionInEditor(7);

            ViewDefaults()
                .Property(x => x.DataService.Name)
                .Property(x => x.DbSchema)
                .Property(x => x.DbObject)
                .Property(x => x.Name)
                .Property(x => x.DisplayName)
                .Property(x => x.PluralisedDisplayName)
                .OrderBy(x => x.Name);
        }
    }
}
