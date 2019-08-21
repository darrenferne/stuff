using System;
using System.Runtime.InteropServices;

namespace Brady.Common.CrystalWrapper
{
    [Guid("34235428-9EB7-458E-B0D2-DEC205F6F74B")]
    [Flags]
    public enum DisplayFlag
    {
        AllowNone = 0,
        AllowSave = 1,
        AllowGrouping = 2,
        AllowNavigate = 4,
        AllowPrint = 8,
        AllowRefresh = 16,
        AllowSearch = 32,
        AllowZoom = 64,
        AllowAll = AllowSave + AllowGrouping + AllowNavigate + AllowPrint + AllowRefresh + AllowSearch + AllowZoom
    }
}