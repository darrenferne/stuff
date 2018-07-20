using BWF.DataServices.Metadata.Enums;
using BWF.Enums.Attributes;
using Newtonsoft.Json;

namespace DataServiceDesigner.Domain
{
    [JsonConverter(typeof(RichEnumConverter))]
    public enum DesignerObjectType
    {
        [RichEnum("Table", "Table")]
        Table,
        [RichEnum("View", "View")]
        View
    }
}