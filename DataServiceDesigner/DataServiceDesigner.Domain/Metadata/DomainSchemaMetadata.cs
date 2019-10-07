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

            StringProperty(x => x.Name)
                .PositionInEditor(2)
                .Parameter(p => p
                    .Query("DomainSchemas?$orderby=Name")
                    .DisplayProperty(o => o.Name)
                    .AllowNullOrEmpty()
                    .AvailableOperators(Operator.Equals));

            ViewDefaults()
                .Property(x => x.Name)
                .OrderBy(x => x.Name);
        }
    }
}
