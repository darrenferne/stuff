using System;

namespace WindowsFormsApplication1
{
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