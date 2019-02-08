// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.OPEN_VIRTUAL_DISK_FLAG
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    [Flags]
    public enum OPEN_VIRTUAL_DISK_FLAG
    {
        OPEN_VIRTUAL_DISK_FLAG_NONE = 0,
        OPEN_VIRTUAL_DISK_FLAG_NO_PARENTS = 1,
        OPEN_VIRTUAL_DISK_FLAG_BLANK_FILE = 2,
        OPEN_VIRTUAL_DISK_FLAG_BOOT_DRIVE = 4
    }
}