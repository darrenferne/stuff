using BWF.DataServices.Metadata.Fluent.Abstract;

namespace DataServiceDesigner.Domain
{
    public class DomainObjectReferencePropertyMetadata : TypeMetadataProvider<DomainObjectReferenceProperty>
    {
        public DomainObjectReferencePropertyMetadata()
        {
            AutoUpdatesByDefault();
            SupportsEditMode();

            DisplayName("Domain Object Reference Property");
            PluralisedDisplayName("Domain Object Reference Properties");

            IntegerProperty(x => x.Id)
                .IsId()
                .IsHiddenInEditor()
                .IsNotEditableInGrid();

            TypeProperty(x => x.Reference)
                .DisplayName("Reference")
                .PositionInEditor(1)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjectReferences?$Expands=Object$orderby=ReferenceName'")
                .DisplayFieldInEditorChoice("ReferenceName")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode();

            TypeProperty(x => x.Parent)
                .DisplayName("Parent Property")
                .PositionInEditor(1)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjectPropertys?$Expand=Object$orderby=PropertyName'")
                .FilteredOn(@"Reference/Parent/Id", @"Object/Id")
                .DisplayFieldInEditorChoice("PropertyName")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode();

            TypeProperty(x => x.Child)
                .DisplayName("Child Property")
                .PositionInEditor(2)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjectPropertys?$Expands=Object$orderby=PropertyName'")
                .DisplayFieldInEditorChoice("PropertyName")
                .FilteredOn(@"Reference/Child/Id", @"Object/Id")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode();

            ExpandsForEdit()
                .Property(x => x.Parent)
                .Property(x => x.Child);

            ViewDefaults()
                .Property(x => x.Reference.ReferenceName)
                .Property(x => x.Reference.ConstraintName)
                .Property(x => x.Parent.Object.ObjectName)
                .Property(x => x.Parent.PropertyName)
                .OrderBy(x => x.Reference.ReferenceName)
                .OrderBy(x => x.Parent.Object.ObjectName)
                .OrderBy(x => x.Parent.PropertyName);
        }
    }
}
