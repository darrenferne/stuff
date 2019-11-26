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

            IntegerProperty(x => x.ReferenceId)
                .DisplayName("Reference")
                .IsHiddenInEditor()
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjectReferences?$Expands=Object$orderby=ReferenceName'")
                .DisplayFieldInEditorChoice("ReferenceName")
                .ValueFieldInEditorChoice("Id");

            TypeProperty(x => x.Reference)
                .DisplayName("Reference")
                .PositionInEditor(1)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjectReferences?$Expands=Object$orderby=ReferenceName'")
                .DisplayFieldInEditorChoice("ReferenceName")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode();

            TypeProperty(x => x.ParentProperty)
                .DisplayName("Parent Property")
                .IsHiddenInEditor()
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjectPropertys?$Expand=Object$orderby=PropertyName'")
                .ValueFieldInEditorChoice("Id")
                .DisplayFieldInEditorChoice("PropertyName");

            IntegerProperty(x => x.ParentPropertyId)
                .DisplayName("Parent Property")
                .PositionInEditor(1)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjectPropertys?$Expand=Object$orderby=PropertyName'")
                .DisplayFieldInEditorChoice("PropertyName")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode();

            IntegerProperty(x => x.ChildProperty)
                .DisplayName("Child Property")
                .IsHiddenInEditor()
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjectPropertys?$Expands=Object$orderby=PropertyName'")
                .DisplayFieldInEditorChoice("PropertyName")
                .ValueFieldInEditorChoice("Id");

            IntegerProperty(x => x.ChildPropertyId)
                .DisplayName("Child Property")
                .PositionInEditor(2)
                .PopulateChoiceQuery("'dataservicedesigner/query/DomainObjectPropertys?$Expands=Object$orderby=PropertyName'")
                .DisplayFieldInEditorChoice("PropertyName")
                .ValueFieldInEditorChoice("Id")
                .IsMandatoryInEditMode();

            ViewDefaults()
                .Property(x => x.Reference.ReferenceName)
                .Property(x => x.Reference.ConstraintName)
                .Property(x => x.ParentProperty.Object.ObjectName)
                .Property(x => x.ParentProperty.PropertyName)
                .Property(x => x.ChildProperty.Object.ObjectName)
                .Property(x => x.ChildProperty.PropertyName)
                .OrderBy(x => x.Reference.ReferenceName)
                .OrderBy(x => x.ParentProperty.Object.ObjectName)
                .OrderBy(x => x.ParentProperty.PropertyName);
        }
    }
}
