// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.VirtualDiskLib
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System.Runtime.InteropServices;
using System.Text;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public static class VirtualDiskLib
    {

        [DllImport("Virtdisk.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int OpenVirtualDisk(ref VIRTUAL_STORAGE_TYPE VirtualStorageType,
            [MarshalAs(UnmanagedType.LPWStr)] string Path, VIRTUAL_DISK_ACCESS_MASK VirtualDiskAccessMask,
            OPEN_VIRTUAL_DISK_FLAG Flags, ref OPEN_VIRTUAL_DISK_PARAMETERS Parameters, ref nint Handle);


        [DllImport("Virtdisk.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int OpenVirtualDisk(ref VIRTUAL_STORAGE_TYPE VirtualStorageType,
            [MarshalAs(UnmanagedType.LPWStr)] string Path, VIRTUAL_DISK_ACCESS_MASK VirtualDiskAccessMask,
            OPEN_VIRTUAL_DISK_FLAG Flags, nint Parameters, ref nint Handle);

        [DllImport("Virtdisk.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            SetLastError = true)]
        public static extern int GetVirtualDiskPhysicalPath(nint VirtualDiskHandle, ref int DiskPathSizeInBytes,
            StringBuilder DiskPath);


        [DllImport("Virtdisk.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int AttachVirtualDisk(nint VirtualDiskHandle, nint SecurityDescriptor,
            ATTACH_VIRTUAL_DISK_FLAG Flags, uint ProviderSpecificFlags, ref ATTACH_VIRTUAL_DISK_PARAMETERS Parameters,
            nint Overlapped);


        [DllImport("Virtdisk.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int DetachVirtualDisk(nint VirtualDiskHandle, DETACH_VIRTUAL_DISK_FLAG Flags,
            uint ProviderSpecificFlags);


        [DllImport("Virtdisk.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int CreateVirtualDisk(ref VIRTUAL_STORAGE_TYPE VirtualStorageType,
            [MarshalAs(UnmanagedType.LPWStr)] string Path, VIRTUAL_DISK_ACCESS_MASK VirtualDiskAccessMask,
            nint SecurityDescriptor, CREATE_VIRTUAL_DISK_FLAG Flags, uint ProviderSpecificFlags,
            ref CREATE_VIRTUAL_DISK_PARAMETERS Parameters, nint Overlapped, ref nint Handle);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(nint hObject);
    }
}