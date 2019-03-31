using BWF.DataServices.Metadata.Fluent.Abstract;

namespace Brady.Limits.Domain.Metadata
{
    public class WorkflowLevelMetadata : TypeMetadataProvider<WorkflowLevel>
    {
        public WorkflowLevelMetadata()
        {
            DisplayName("Workflow Level");

            IntegerProperty(p => p.Id)
                .IsId()
                .IsHidden()
                .IsHiddenInEditor();

            StringProperty(p => p.Name)
                .DisplayName("Name");

            ViewDefaults()
                .Property(p => p.Name)
                .OrderBy(p => p.Name);
        }
    }
}