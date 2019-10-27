using BWF.DataServices.Metadata.Fluent.Abstract;
using BWF.DataServices.Metadata.Fluent.Enums;

namespace DataServiceDesigner.Domain
{
    public class DomainObjectReferenceMetadata : TypeMetadataProvider<DomainObjectReference>
    {
        public DomainObjectReferenceMetadata()
        {
            AutoUpdatesByDefault();
            SupportsEditMode();

            DisplayName("Domain Object Reference");

            IntegerProperty(x => x.Id)
                .IsId()
                .IsHiddenInEditor()
                .IsNotEditableInGrid();

            TypeProperty(x => x.Schema)
                .DisplayName("Schema")
                .PositionInEditor(1)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainSchemas?$orderby=SchemaName'")
                .DisplayFieldInEditorChoice("SchemaName")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode();

            StringProperty(x => x.ReferenceName)
                 .DisplayName("Reference Name")
                 .PositionInEditor(1);

            StringProperty(x => x.ConstraintName)
                 .DisplayName("Constraint Name")
                 .PositionInEditor(2);

            EnumProperty(x => x.ReferenceType)
                .DisplayName("Reference Type")
                .PositionInEditor(3);

            TypeProperty(x => x.Parent)
                .DisplayName("Parent Object")
                .PositionInEditor(4)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjects?$orderby=ObjectName'")
                //.FilteredOn("Schema/Id","Schema/Id")
                .DisplayFieldInEditorChoice("ObjectName")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode()
                .CustomControl("cc-parentObject")
                .CustomControlHeight(30);

            TypeProperty(x => x.Child)
                .DisplayName("Child Object")
                .PositionInEditor(5)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjects?$orderby=ObjectName'")
                //.FilteredOn("Schema/Id", "Schema/Id")
                .DisplayFieldInEditorChoice("ObjectName")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode()
                .CustomControl("cc-childObject")
                .CustomControlHeight(30);

            CollectionProperty(x => x.Properties)
                .DisplayName("Properties")
                .PositionInEditor(6)
                .CustomControl("cc-domainObjectReferenceProperties")
                .CustomControlHeight(180);

            ExpandsForEdit()
                .Property(x => x.Parent.Properties)
                .Property(x => x.Child.Properties)
                .Property(x => x.Properties[0].ParentProperty)
                .Property(x => x.Properties[0].ChildProperty);

            ViewDefaults()
                .Property(x => x.ReferenceName)
                .Property(x => x.ConstraintName)
                .Property(x => x.ReferenceType)
                .OrderBy(x => x.ReferenceName);
        }
    }
}
