using BWF.DataServices.Metadata.Fluent.Abstract;

namespace Brady.Limits.Domain.Metadata
{
    public class SelectionMetadata : TypeMetadataProvider<Selection>
    {
        public SelectionMetadata()
        {
            DisplayName("Selection");

            AutoUpdatesByDefault();

            IntegerProperty(p => p.Id)
                .IsId()
                .IsHidden()
                .IsHiddenInEditor();

            StringProperty(p => p.Name)
                .DisplayName("Name");

            StringProperty(p => p.Filter)
                .DisplayName("Filter");

            ViewDefaults()
                .Property(p => p.Name)
                .Property(p => p.Filter)
                .OrderBy(p => p.Name);
        }
    }
}