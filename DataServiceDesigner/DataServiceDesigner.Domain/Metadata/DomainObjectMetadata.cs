using BWF.DataServices.Metadata.Fluent.Abstract;
using BWF.DataServices.Metadata.Fluent.Enums;

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
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainDataServices?$orderby=Name'")
                .DisplayFieldInEditorChoice("Name")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode();

            TypeProperty(x => x.Schema)
                .DisplayName("Schema")
                .PositionInEditor(2)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainSchemas?$orderby=Name'")
                .DisplayFieldInEditorChoice(x => x.Name)
                .ValueFieldInEditorChoice(x => x.Id)
                .IsMandatoryInEditMode();

            StringProperty(x => x.DbName)
                 .DisplayName("Db Name")
                 .PositionInEditor(3);

            StringProperty(x => x.Name)
                .PositionInEditor(4)
                .Parameter(p => p
                    .Query("DomainObjects?$orderby=Name")
                    .DisplayProperty(o => o.Name)
                    .AllowNullOrEmpty()
                    .AvailableOperators(Operator.Equals));

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
                .Property(x => x.Schema.Name)
                .Property(x => x.DbName)
                .Property(x => x.Name)
                .Property(x => x.DisplayName)
                .Property(x => x.PluralisedDisplayName)
                .OrderBy(x => x.Name);
        }
    }
}
