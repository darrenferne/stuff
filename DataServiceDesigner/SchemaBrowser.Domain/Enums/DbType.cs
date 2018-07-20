using BWF.DataServices.Metadata.Enums;
using BWF.Enums.Attributes;
using Newtonsoft.Json;

namespace SchemaBrowser.Domain
{
    [JsonConverter(typeof(RichEnumConverter))]
    public enum DbType
    {
        [RichEnum("SQL Server", "SQLServer")]
        SqlServer,
        [RichEnum("Oracle", "Oracle")]
        Oracle
    }
}