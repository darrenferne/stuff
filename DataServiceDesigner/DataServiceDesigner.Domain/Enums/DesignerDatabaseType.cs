using BWF.DataServices.Metadata.Enums;
using BWF.Enums.Attributes;
using Newtonsoft.Json;

namespace DataServiceDesigner.Domain
{
    [JsonConverter(typeof(RichEnumConverter))]
    public enum DesignerDatabaseType
    {
        [RichEnum("SQL Server", "SQLServer")]
        SqlServer,
        [RichEnum("Oracle", "Oracle")]
        Oracle
    }
}