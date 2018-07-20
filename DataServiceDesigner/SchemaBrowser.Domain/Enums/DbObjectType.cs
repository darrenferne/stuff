using BWF.DataServices.Metadata.Enums;
using BWF.Enums.Attributes;
using Newtonsoft.Json;

namespace SchemaBrowser.Domain
{
    [JsonConverter(typeof(RichEnumConverter))]
    public enum DbObjectType
    {
        [RichEnum("Table", "Table")]
        Table,
        [RichEnum("View", "View")]
        View
    }
}