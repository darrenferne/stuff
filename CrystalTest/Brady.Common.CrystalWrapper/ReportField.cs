using System.Runtime.InteropServices;

namespace Brady.Common.CrystalWrapper
{
    [Guid("8F09FAA8-9ABC-497D-85A9-FEC37A87B62E")]
    public class ReportField
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string TableName { get; set; }
    }
}