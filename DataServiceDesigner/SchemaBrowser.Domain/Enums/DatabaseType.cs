using BWF.DataServices.Metadata.Enums;
using BWF.Enums.Attributes;
using Newtonsoft.Json;

namespace SchemaBrowser.Domain
{
    [JsonConverter(typeof(RichEnumConverter))]
    public enum DatabaseType
    {
        [RichEnum("SQL Server", "SQLServer")]
        SqlServer,
        [RichEnum("Oracle", "Oracle")]
        Oracle
    }
}