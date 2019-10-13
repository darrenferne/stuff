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

            TypeProperty(x => x.Schema)
                .DisplayName("Schema")
                .PositionInEditor(2)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainSchemas?$orderby=Name'")
                .DisplayFieldInEditorChoice(x => x.ObjectName)
                .ValueFieldInEditorChoice(x => x.Id)
                .IsMandatoryInEditMode();

            StringProperty(x => x.TableName)
                 .DisplayName("Table Name")
                 .PositionInEditor(3);

            StringProperty(x => x.ObjectName)
                .PositionInEditor(4)
                .Parameter(p => p
                    .Query("DomainObjects?$orderby=Name")
                    .DisplayProperty(o => o.ObjectName)
                    .AllowNullOrEmpty()
                    .AvailableOperators(Operator.Equals));

            StringProperty(x => x.DisplayName)
                 .DisplayName("Display Name")
                 .PositionInEditor(5);

            StringProperty(x => x.PluralisedDisplayName)
                 .DisplayName("Pluralised Display Name")
                 .PositionInEditor(6);
            
            CollectionProperty(x => x.Properties)
                .DisplayName("Properties")
                .PositionInEditor(7);

            ViewDefaults()
                .Property(x => x.Schema.DataService.Name)
                .Property(x => x.Schema.SchemaName)
                .Property(x => x.TableName)
                .Property(x => x.ObjectName)
                .Property(x => x.DisplayName)
                .Property(x => x.PluralisedDisplayName)
                .OrderBy(x => x.ObjectName);

            //ExpandsForEdit()
            //    .Property(p => p.Properties);
            //    .Property(p => p.Properties[0].Object);
        }
    }
}
