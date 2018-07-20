using BWF.DataServices.Metadata.Enums;
using BWF.Enums.Attributes;
using Newtonsoft.Json;

namespace SchemaBrowser.Domain
{
    [JsonConverter(typeof(RichEnumConverter))]
    public enum DbConnectionStatus
    {
        [RichEnum("ValidationPending", "Validation Pending")]
        ValidationPending,
        [RichEnum("Valid", "Valid")]
        Valid,
        [RichEnum("Invalid", "Invalid")]
        Invalid,
        [RichEnum("SchemaPending", "Schema Pending")]
        SchemaPending,
        [RichEnum("SchemaAvailable", "Schema Available")]
        SchemaAvailable
    }
}