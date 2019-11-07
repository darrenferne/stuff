using BWF.DataServices.Metadata.Fluent.Abstract;
using BWF.DataServices.Metadata.Fluent.Enums;

namespace DataServiceDesigner.Domain
{
    public class DataServiceSolutionMetadata : TypeMetadataProvider<DataServiceSolution>
    {
        public DataServiceSolutionMetadata()
        {
            AutoUpdatesByDefault();
            SupportsEditMode();
            
            DisplayName("Data Service Solution");

            IntegerProperty(x => x.Id)
                .IsId()
                .IsHiddenInEditor()
                .IsNotEditableInGrid();

            StringProperty(x => x.Name)
                .PositionInEditor(1)
                .DisplayName("Name");

            StringProperty(x => x.CompanyName)
                .PositionInEditor(2)
                .DisplayName("Company Name")
                .CustomControl("cc-stringWithDefault", "{ \"default\": \"" + Defaults.CompanyName + "\" }")
                .CustomControlHeight(30);

            StringProperty(x => x.NamespacePrefix)
                .PositionInEditor(3)
                .DisplayName("Namespace Prefix")
                .CustomControl("cc-stringWithDefault", "{ \"default\": \"" + Defaults.NamespacePrefix + "\" }")
                .CustomControlHeight(30);

            StringProperty(x => x.ServiceName)
                .PositionInEditor(4)
                .DisplayName("Service Name")
                .CustomControl("cc-stringWithDependency", @"
                {
                    ""dependency"": ""Name"", 
                    ""action"": ""this.property(this.dependency() + 'Service');""
                }") 
                .CustomControlHeight(30);

            StringProperty(x => x.ServiceDisplayName)
                .PositionInEditor(5)
                .DisplayName("Service Display Name")
                .CustomControl("cc-stringWithDependency", @"
                {
                    ""dependency"": ""Name"", 
                    ""action"": ""this.property(this.dependency() + ' Service');""
                }")
                .CustomControlHeight(30);

            StringProperty(x => x.ServiceDescription)
                .PositionInEditor(6)
                .DisplayName("Service Description")
                .CustomControl("cc-stringWithDependency", @"
                {
                    ""dependency"": ""Name"", 
                    ""action"": ""this.property(this.dependency() + ' Service');""
                }")
                .HeightInLines(2)
                .CustomControlHeight(60);


            CollectionProperty(x => x.DataServices)
                .PositionInEditor(7);

            ViewDefaults()
                .Property(x => x.Name)
                .Property(x => x.NamespacePrefix)
                .Property(x => x.ServiceName)
                .Property(x => x.ServiceDisplayName)
                .Property(x => x.ServiceDescription)
                .Property(x => x.CompanyName)
                .OrderBy(x => x.Name);
        }
    }
}
