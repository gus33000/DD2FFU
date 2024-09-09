using System;
using System.Runtime.InteropServices;

namespace DD2FFU
{
    internal class NativeMethods
    {
        public enum ATTACH_VIRTUAL_DISK_FLAG
        {
            ATTACH_VIRTUAL_DISK_FLAG_NONE = 0x00000000,
            ATTACH_VIRTUAL_DISK_FLAG_READ_ONLY = 0x00000001,
            ATTACH_VIRTUAL_DISK_FLAG_NO_DRIVE_LETTER = 0x00000002,
            ATTACH_VIRTUAL_DISK_FLAG_PERMANENT_LIFETIME = 0x00000004,
            ATTACH_VIRTUAL_DISK_FLAG_NO_LOCAL_HOST = 0x00000008
        }

        public enum ATTACH_VIRTUAL_DISK_VERSION
        {
            ATTACH_VIRTUAL_DISK_VERSION_UNSPECIFIED = 0,
            ATTACH_VIRTUAL_DISK_VERSION_1 = 1
        }

        public enum OPEN_VIRTUAL_DISK_FLAG
        {
            OPEN_VIRTUAL_DISK_FLAG_NONE = 0x00000000,
            OPEN_VIRTUAL_DISK_FLAG_NO_PARENTS = 0x00000001,
            OPEN_VIRTUAL_DISK_FLAG_BLANK_FILE = 0x00000002,
            OPEN_VIRTUAL_DISK_FLAG_BOOT_DRIVE = 0x00000004
        }

        public enum OPEN_VIRTUAL_DISK_VERSION
        {
            OPEN_VIRTUAL_DISK_VERSION_1 = 1
        }

        public enum VIRTUAL_DISK_ACCESS_MASK
        {
            VIRTUAL_DISK_ACCESS_ATTACH_RO = 0x00010000,
            VIRTUAL_DISK_ACCESS_ATTACH_RW = 0x00020000,
            VIRTUAL_DISK_ACCESS_DETACH = 0x00040000,
            VIRTUAL_DISK_ACCESS_GET_INFO = 0x00080000,
            VIRTUAL_DISK_ACCESS_CREATE = 0x00100000,
            VIRTUAL_DISK_ACCESS_METAOPS = 0x00200000,
            VIRTUAL_DISK_ACCESS_READ = 0x000d0000,
            VIRTUAL_DISK_ACCESS_ALL = 0x003f0000,
            VIRTUAL_DISK_ACCESS_WRITABLE = 0x00320000
        }

        public const int ERROR_SUCCESS = 0;
        public const int OPEN_VIRTUAL_DISK_RW_DEPTH_DEFAULT = 1;
        public const int VIRTUAL_STORAGE_TYPE_DEVICE_VHD = 2;

        public static readonly Guid VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT =
            new("EC984AEC-A0F9-47e9-901F-71415A66345B");

        [DllImport("virtdisk.dll", CharSet = CharSet.Unicode)]
        public static extern int AttachVirtualDisk(IntPtr VirtualDiskHandle, IntPtr SecurityDescriptor,
            ATTACH_VIRTUAL_DISK_FLAG Flags, int ProviderSpecificFlags, ref ATTACH_VIRTUAL_DISK_PARAMETERS Parameters,
            IntPtr Overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("virtdisk.dll", CharSet = CharSet.Unicode)]
        public static extern int OpenVirtualDisk(ref VIRTUAL_STORAGE_TYPE VirtualStorageType, string Path,
            VIRTUAL_DISK_ACCESS_MASK VirtualDiskAccessMask, OPEN_VIRTUAL_DISK_FLAG Flags,
            ref OPEN_VIRTUAL_DISK_PARAMETERS Parameters, ref IntPtr Handle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ATTACH_VIRTUAL_DISK_PARAMETERS
        {
            public ATTACH_VIRTUAL_DISK_VERSION Version;
            public ATTACH_VIRTUAL_DISK_PARAMETERS_Version1 Version1;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ATTACH_VIRTUAL_DISK_PARAMETERS_Version1
        {
            public int Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OPEN_VIRTUAL_DISK_PARAMETERS
        {
            public OPEN_VIRTUAL_DISK_VERSION Version;
            public OPEN_VIRTUAL_DISK_PARAMETERS_Version1 Version1;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OPEN_VIRTUAL_DISK_PARAMETERS_Version1
        {
            public int RWDepth;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct VIRTUAL_STORAGE_TYPE
        {
            public int DeviceId;
            public Guid VendorId;
        }
    }
}