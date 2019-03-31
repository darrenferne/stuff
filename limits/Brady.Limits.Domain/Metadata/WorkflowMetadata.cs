using BWF.DataServices.Metadata.Fluent.Abstract;

namespace Brady.Limits.Domain.Metadata
{
    public class WorkflowMetadata : TypeMetadataProvider<Workflow>
    {
        public WorkflowMetadata()
        {
            DisplayName("Workflow");

            AutoUpdatesByDefault();

            IntegerProperty(p => p.Id)
                .IsId()
                .IsHidden()
                .IsHiddenInEditor();

            StringProperty(p => p.Name)
                .DisplayName("Name")
                .PositionInEditor(1);

            CollectionProperty(p => p.Levels)
                .DisplayName("Levels")
                .CustomControl("cc-workflowlevels")
                .CustomControlHeight(300)
                .PositionInEditor(2);

            ViewDefaults()
                .Property(p => p.Name)
                .OrderBy(p => p.Name);
        }
    }
}