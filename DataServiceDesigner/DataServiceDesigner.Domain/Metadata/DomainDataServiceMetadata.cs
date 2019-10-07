using BWF.DataServices.Metadata.Fluent.Abstract;
using BWF.DataServices.Metadata.Fluent.Enums;
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
                .PositionInEditor(1)
                .Parameter(p => p
                    .Query("DomainDataServices?$orderby=Name")
                    .DisplayProperty(o => o.Name)
                    .AllowNullOrEmpty()
                    .AvailableOperators(Operator.Equals));

            TypeProperty(p => p.Connection)
                .DisplayName("Connection")
                .PositionInEditor(2)
                .PopulateChoiceQuery("'dataservicedesigner/query/DataServiceConnections?$orderby=Name'")
                .DisplayFieldInEditorChoice(x => x.Name)
                .ValueFieldInEditorChoice(x => x.Id)
                .IsNotEditableInGrid();

            TypeProperty(x => x.DefaultSchema)
                .DisplayName("Default Schema")
                .PositionInEditor(3)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainSchemas?$orderby=Name'")
                .DisplayFieldInEditorChoice(x => x.Name)
                .ValueFieldInEditorChoice(x => x.Id)
                .IsNotEditableInGrid();
            
            CollectionProperty(x => x.DomainObjects)
                .PositionInEditor(4)
                .DisplayName("Domain Objects")
                .CustomControl("cc-domainObjects")
                .CustomControlHeight(400);

            ExpandsForEdit()
                .Property(p => p.Connection)
                .Property(p => p.DefaultSchema)
                .Property(p => p.DomainObjects[0].DataService)
                .Property(p => p.DomainObjects[0].Schema);

            ViewDefaults()
                .Property(x => x.Name)
                .OrderBy(x => x.Name);
        }
    }
}
