// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.SecurityInformationFlags
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    
    public enum SecurityInformationFlags : uint
    {
        OWNER_SECURITY_INFORMATION = 1,
        GROUP_SECURITY_INFORMATION = 2,
        DACL_SECURITY_INFORMATION = 4,
        SACL_SECURITY_INFORMATION = 8,
        MANDATORY_ACCESS_LABEL = 16, // 0x00000010
        UNPROTECTED_SACL_SECURITY_INFORMATION = 268435456, // 0x10000000
        UNPROTECTED_DACL_SECURITY_INFORMATION = 536870912, // 0x20000000
        PROTECTED_SACL_SECURITY_INFORMATION = 1073741824, // 0x40000000
        PROTECTED_DACL_SECURITY_INFORMATION = 2147483648 // 0x80000000
    }
}