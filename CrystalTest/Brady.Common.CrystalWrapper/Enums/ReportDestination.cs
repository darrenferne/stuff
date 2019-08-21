using System.Runtime.InteropServices;

namespace Brady.Common.CrystalWrapper
{
    [Guid("D1C95E90-5534-4995-BEC4-BB77DAB00413")]
    public enum ReportDestination : int
    {
        None = 0,
        Disk = 1,
        Email = 2
    }
}