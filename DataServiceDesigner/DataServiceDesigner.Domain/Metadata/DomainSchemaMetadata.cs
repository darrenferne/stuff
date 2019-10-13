using BWF.DataServices.Metadata.Fluent.Abstract;
using BWF.DataServices.Metadata.Fluent.Enums;

namespace DataServiceDesigner.Domain
{
    public class DomainSchemaMetadata : TypeMetadataProvider<DomainSchema>
    {
        public DomainSchemaMetadata()
        {
            AutoUpdatesByDefault();
            SupportsEditMode();
            
            DisplayName("Domain Schema");
            
            IntegerProperty(x => x.Id)
                .IsId()
                .IsHiddenInEditor()
                .IsNotEditableInGrid();
            
            TypeProperty(x => x.DataService)
                .PositionInEditor(2)
                .DisplayName("Data Service")
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainDataServices?$expands=Connection&$orderby=Name'")
                .ValueFieldInEditorChoice(nameof(DomainDataService.Id))
                .DisplayFieldInEditorChoice(nameof(DomainDataService.Name))
                .IsMandatoryInEditMode();
            
            StringProperty(x => x.SchemaName)
                .PositionInEditor(3)
                .Parameter(p => p
                    .Query("DomainSchemas?$orderby=Name")
                    .DisplayProperty(o => o.SchemaName)
                    .AllowNullOrEmpty()
                    .AvailableOperators(Operator.Equals));

            BooleanProperty(x => x.IsDefault)
                .PositionInEditor(3)
                .DisplayName("Is Default");

            CollectionProperty(x => x.Objects)
                .PositionInEditor(4)
                .DisplayName("Objects")
                .CustomControl("cc-domainObjects")
                .CustomControlHeight(300);

            ViewDefaults()
                .Property(x => x.SchemaName)
                .Property(x => x.IsDefault)
                .OrderBy(x => x.SchemaName);

            ExpandsForEdit()
                .Property(p => p.DataService)
                .Property(p => p.Objects);
            ////    .Property(p => p.Objects[0].Properties[0].Object);
        }
    }
}
