using BWF.DataServices.Metadata.Fluent.Abstract;
using BWF.DataServices.Metadata.Fluent.Enums;

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

            TypeProperty(p => p.Solution)
                .DisplayName("Solution")
                .PositionInEditor(1)
                .PopulateChoiceQuery("'dataservicedesigner/query/DataServiceSolutions?$orderby=Name'")
                .DisplayFieldInEditorChoice(nameof(DataServiceConnection.Name))
                .ValueFieldInEditorChoice(nameof(DataServiceConnection.Id))
                .IsNotEditableInGrid();

            StringProperty(x => x.Name)
                .PositionInEditor(2)
                .DisplayName("Data Service Name")
                .Parameter(p => p
                    .Query("DomainDataServices?$orderby=Name")
                    .DisplayProperty(o => o.Name)
                    .AllowNullOrEmpty()
                    .AvailableOperators(Operator.Equals));

            TypeProperty(p => p.Connection)
                .DisplayName("Connection")
                .PositionInEditor(3)
                .PopulateChoiceQuery("'dataservicedesigner/query/DataServiceConnections?$orderby=Name'")
                .DisplayFieldInEditorChoice(nameof(DataServiceConnection.Name))
                .ValueFieldInEditorChoice(nameof(DataServiceConnection.Id))
                .IsNotEditableInGrid();

            CollectionProperty(x => x.Schemas)
                .PositionInEditor(4)
                .DisplayName("Schemas")
                .CustomControl("cc-domainSchemas")
                .CustomControlHeight(300);

            ViewDefaults()
                .Property(x => x.Solution.Name)
                .Property(x => x.Name)
                .OrderBy(x => x.Solution.Name)
                .OrderBy(x => x.Name);
        }
    }
}
