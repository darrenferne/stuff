using System.Runtime.InteropServices;

namespace Brady.Common.CrystalWrapper
{
    [Guid("A7E744B0-65F0-4977-AD47-F09CC6DE7119")]
    public enum ReportFormat : int
    {
        None = 0,
        RPT = 1,
        RTF = 2,
        Word = 3,
        Excel = 4,
        PDF = 5,
        HTML32 = 6,
        HTML40 = 7,
        Text = 9,
        CSV = 10,
        TSV = 11,
        XML = 14
    }
}