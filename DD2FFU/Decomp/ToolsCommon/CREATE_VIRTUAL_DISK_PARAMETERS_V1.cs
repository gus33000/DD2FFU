// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.CREATE_VIRTUAL_DISK_PARAMETERS_V1
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.Runtime.InteropServices;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    
    public struct CREATE_VIRTUAL_DISK_PARAMETERS_V1
    {
        public Guid UniqueId;
        public ulong MaximumSize;
        public uint BlockSizeInBytes;
        public uint SectorSizeInBytes;
        [MarshalAs(UnmanagedType.LPWStr)] public string ParentPath;
        [MarshalAs(UnmanagedType.LPWStr)] public string SourcePath;
    }
}