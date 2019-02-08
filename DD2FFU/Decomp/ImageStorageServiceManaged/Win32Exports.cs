// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.Win32Exports
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public sealed class Win32Exports
    {
        [Flags]
        
        public enum AllocationType : uint
        {
            MEM_COMMIT = 4096, // 0x00001000
            MEM_RESERVE = 8192, // 0x00002000
            MEM_RESET = 524288, // 0x00080000
            MEM_LARGE_PAGES = 536870912, // 0x20000000
            MEM_PHYSICAL = 4194304, // 0x00400000
            MEM_TOP_DOWN = 1048576, // 0x00100000
            MEM_WRITE_WATCH = 2097152 // 0x00200000
        }

        
        public enum CreationDisposition : uint
        {
            CREATE_NEW = 1,
            CREATE_ALWAYS = 2,
            OPEN_EXISTING = 3,
            OPEN_ALWAYS = 4,
            TRUNCATE_EXSTING = 5
        }

        [Flags]
        
        public enum DesiredAccess : uint
        {
            GENERIC_READ = 2147483648, // 0x80000000
            GENERIC_WRITE = 1073741824 // 0x40000000
        }

        [Flags]
        
        public enum FlagsAndAttributes : uint
        {
            FILE_ATTRIBUTES_ARCHIVE = 32, // 0x00000020
            FILE_ATTRIBUTE_HIDDEN = 2,
            FILE_ATTRIBUTE_NORMAL = 128, // 0x00000080
            FILE_ATTRIBUTE_OFFLINE = 4096, // 0x00001000
            FILE_ATTRIBUTE_READONLY = 1,
            FILE_ATTRIBUTE_SYSTEM = 4,
            FILE_ATTRIBUTE_TEMPORARY = 256, // 0x00000100
            FILE_FLAG_WRITE_THROUGH = 2147483648, // 0x80000000
            FILE_FLAG_OVERLAPPED = 1073741824, // 0x40000000
            FILE_FLAG_NO_BUFFERING = 536870912, // 0x20000000
            FILE_FLAG_RANDOM_ACCESS = 268435456, // 0x10000000
            FILE_FLAG_SEQUENTIAL_SCAN = 134217728, // 0x08000000
            FILE_FLAG_DELETE_ON = 67108864, // 0x04000000
            FILE_FLAG_POSIX_SEMANTICS = 16777216, // 0x01000000
            FILE_FLAG_OPEN_REPARSE_POINT = 2097152, // 0x00200000
            FILE_FLAG_OPEN_NO_CALL = 1048576 // 0x00100000
        }

        [Flags]
        
        public enum FreeType : uint
        {
            MEM_DECOMMIT = 16384, // 0x00004000
            MEM_RELEASE = 32768 // 0x00008000
        }

        
        public enum IoctlControlCode : uint
        {
            IoctlDiskGetDriveLayoutEx = 458832, // 0x00070050
            IoctlMountManagerScrubRegistry = 7192632 // 0x006DC038
        }

        [Flags]
        
        public enum MemoryProtection : uint
        {
            PAGE_EXECUTE = 16, // 0x00000010
            PAGE_EXECUTE_READ = 32, // 0x00000020
            PAGE_EXECUTE_READWRITE = 64, // 0x00000040
            PAGE_EXECUTE_WRITECOPY = 128, // 0x00000080
            PAGE_NOACCESS = 1,
            PAGE_READONLY = 2,
            PAGE_READWRITE = 4,
            PAGE_WRITECOPY = 8,
            PAGE_GUARD = 256, // 0x00000100
            PAGE_NOCACHE = 512, // 0x00000200
            PAGE_WRITECOMBINE = 1024 // 0x00000400
        }

        
        public enum MoveMethod : uint
        {
            FILE_BEGIN,
            FILE_CURRENT,
            FILE_END
        }

        public enum PARTITION_STYLE
        {
            MasterBootRecord,
            GuidPartitionTable,
            Raw
        }

        [Flags]
        
        public enum PartitionAttributes : ulong
        {
            GPT_ATTRIBUTE_PLATFORM_REQUIRED = 1,
            GPT_BASIC_DATA_ATTRIBUTE_NO_DRIVE_LETTER = 9223372036854775808, // 0x8000000000000000
            GPT_BASIC_DATA_ATTRIBUTE_HIDDEN = 4611686018427387904, // 0x4000000000000000
            GPT_BASIC_DATA_ATTRIBUTE_SHADOW_COPY = 2305843009213693952, // 0x2000000000000000
            GPT_BASIC_DATA_ATTRIBUTE_READ_ONLY = 1152921504606846976 // 0x1000000000000000
        }

        [Flags]
        
        public enum ShareMode : uint
        {
            FILE_SHARE_NONE = 0,
            FILE_SHARE_READ = 1,
            FILE_SHARE_WRITE = 2,
            FILE_SHARE_DELETE = 4
        }

        public const int S_OK = 0;
        public const int INFINITE = -1;
        private const uint MountManagerControlType = 109;
        private const uint IoctlDiskBase = 7;
        private const uint MethodBuffered = 0;
        private const uint FileReadAccess = 1;
        private const uint FileReadData = 1;
        private const uint FileWriteAccess = 2;
        private const uint FileAnyAccess = 0;
        private const uint DeviceDisk = 7;
        private const uint DiskBase = 7;
        private const uint DiskUserStart = 2016;
        public const int ANYSIZE_ARRAY = 1;
        public const int INVALID_HANDLE_VALUE = -1;

         public const uint SE_PRIVILEGE_ENABLED_BY_DEFAULT = 1;

         public const uint SE_PRIVILEGE_ENABLED = 2;

         public const uint SE_PRIVILEGE_REMOVED = 4;

         public const uint SE_PRIVILEGE_USED_FOR_ACCESS = 2147483648;

         public const uint STANDARD_RIGHTS_REQUIRED = 983040;

         public const uint STANDARD_RIGHTS_READ = 131072;

         public const uint TOKEN_ASSIGN_PRIMARY = 1;

         public const uint TOKEN_DUPLICATE = 2;

         public const uint TOKEN_IMPERSONATE = 4;

         public const uint TOKEN_QUERY = 8;

         public const uint TOKEN_QUERY_SOURCE = 16;

         public const uint TOKEN_ADJUST_PRIVILEGES = 32;

         public const uint TOKEN_ADJUST_GROUPS = 64;

         public const uint TOKEN_ADJUST_DEFAULT = 128;

         public const uint TOKEN_ADJUST_SESSIONID = 256;

         public const uint TOKEN_READ = 131080;

         public const uint TOKEN_ALL_ACCESS = 983551;

         public const uint REG_NONE = 0;

         public const uint REG_SZ = 1;

         public const uint REG_EXPAND_SZ = 2;

         public const uint REG_BINARY = 3;

         public const uint REG_DWORD = 4;

         public const uint REG_DWORD_BIG_ENDIAN = 5;

         public const uint REG_LINK = 6;

         public const uint REG_MULTI_SZ = 7;

         public const uint REG_RESOURCE_LIST = 8;

         public const uint REG_FULL_RESOURCE_DESCRIPTOR = 9;

         public const uint REG_RESOURCE_REQUIREMENTS_LIST = 10;

         public const uint REG_QWORD = 11;

        public static int ERROR_SUCCESS = 0;
        public static int ERROR_NO_MORE_ITEMS = 259;
        public static string MountManagerPath = "\\\\.\\MountPointManager";

        public static bool FAILED(int hr)
        {
            return hr < 0;
        }

        public static bool SUCCEEDED(int hr)
        {
            return hr >= 0;
        }

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle_Native(IntPtr handle);

        public static void CloseHandle(IntPtr handle)
        {
            if (!CloseHandle_Native(handle))
                throw new Win32ExportException(string.Format("{0} failed with error {1}",
                    MethodBase.GetCurrentMethod().Name, Marshal.GetLastWin32Error()));
        }

        [DllImport("kernel32.dll", EntryPoint = "CreateFile", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern SafeFileHandle CreateFile_Native(string fileName, DesiredAccess desiredAccess,
            ShareMode shareMode, IntPtr securityAttributes, CreationDisposition creationDisposition,
            FlagsAndAttributes flagsAndAttributes, IntPtr templateFileHandle);

        
        public static SafeFileHandle CreateFile(string fileName, DesiredAccess desiredAccess, ShareMode shareMode,
            CreationDisposition creationDisposition, FlagsAndAttributes flagsAndAttributes)
        {
            var fileNative = CreateFile_Native(fileName, desiredAccess, shareMode, IntPtr.Zero, creationDisposition,
                flagsAndAttributes, IntPtr.Zero);
            if (!fileNative.IsInvalid)
                return fileNative;
            var lastWin32Error = Marshal.GetLastWin32Error();
            throw new Win32ExportException(string.Format("{0}({1}) failed with error {2}",
                MethodBase.GetCurrentMethod().Name, string.IsNullOrEmpty(fileName) ? "" : fileName, lastWin32Error));
        }

        [DllImport("kernel32.dll", EntryPoint = "ReadFile", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReadFile_Native(SafeFileHandle fileHandle, out byte[] buffer, uint bytesToRead,
            out uint bytesRead, IntPtr overlapped);

        
        public static void ReadFile(SafeFileHandle fileHandle, out byte[] buffer, uint bytesToRead, out uint bytesRead)
        {
            if (!ReadFile_Native(fileHandle, out buffer, bytesToRead, out bytesRead, IntPtr.Zero))
                throw new Win32ExportException(string.Format("ReadFile failed with error: {0}",
                    Marshal.GetLastWin32Error()));
        }

        [DllImport("kernel32.dll", EntryPoint = "ReadFile", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReadFile_Native(SafeFileHandle fileHandle, IntPtr buffer, uint bytesToRead,
            out uint bytesRead, IntPtr overlapped);

        
        public static void ReadFile(SafeFileHandle fileHandle, IntPtr buffer, uint bytesToRead, out uint bytesRead)
        {
            if (!ReadFile_Native(fileHandle, buffer, bytesToRead, out bytesRead, IntPtr.Zero))
                throw new Win32ExportException(string.Format("ReadFile failed with error: {0}",
                    Marshal.GetLastWin32Error()));
        }

        [DllImport("kernel32.dll", EntryPoint = "WriteFile", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WriteFile_Native(SafeFileHandle handle, IntPtr buffer, uint numBytesToWrite,
            out uint numBytesWritten, IntPtr overlapped);

        
        public static void WriteFile(SafeFileHandle fileHandle, IntPtr buffer, uint bytesToWrite, out uint bytesWritten)
        {
            if (!WriteFile_Native(fileHandle, buffer, bytesToWrite, out bytesWritten, IntPtr.Zero))
                throw new Win32ExportException(string.Format("WriteFile failed with error: {0}",
                    Marshal.GetLastWin32Error()));
        }

        [DllImport("kernel32.dll", EntryPoint = "SetFilePointerEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetFilePointerEx_Native(SafeFileHandle fileHandle, long distanceToMove,
            out long newFilePointer, MoveMethod moveMethod);

        
        public static void SetFilePointerEx(SafeFileHandle fileHandle, long distanceToMove, out long newFileLocation,
            MoveMethod moveMethod)
        {
            if (!SetFilePointerEx_Native(fileHandle, distanceToMove, out newFileLocation, moveMethod))
                throw new Win32ExportException(string.Format("SetFilePointerEx failed with error: {0}",
                    Marshal.GetLastWin32Error()));
        }

        [DllImport("kernel32.dll", EntryPoint = "VirtualAlloc", SetLastError = true)]
        private static extern IntPtr VirtualAlloc_Native(IntPtr lpAddress, UIntPtr sizeInBytes,
            AllocationType allocationType, MemoryProtection memoryProtection);

        
        public static IntPtr VirtualAlloc(UIntPtr sizeInBytes, AllocationType allocationType,
            MemoryProtection memoryProtection)
        {
            var num = VirtualAlloc_Native(IntPtr.Zero, sizeInBytes, allocationType, memoryProtection);
            if (!(num == IntPtr.Zero))
                return num;
            throw new Win32ExportException(string.Format("{0} failed with error {1}",
                MethodBase.GetCurrentMethod().Name, Marshal.GetLastWin32Error()));
        }

        [DllImport("kernel32.dll", EntryPoint = "VirtualFree", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool VirtualFree_Native(IntPtr address, UIntPtr sizeInBytes, FreeType freeType);

        
        public static void VirtualFree(IntPtr address, FreeType freeType)
        {
            var zero = UIntPtr.Zero;
            if (!VirtualFree_Native(address, zero, freeType))
                throw new Win32ExportException(string.Format("{0} failed with error {1}",
                    MethodBase.GetCurrentMethod().Name, Marshal.GetLastWin32Error()));
        }

        
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int memcmp(byte[] buffer1, IntPtr buffer2, UIntPtr count);

        [DllImport("kernel32.dll", EntryPoint = "FlushFileBuffers", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlushFileBuffers_Native(SafeFileHandle fileHandle);

        
        public static void FlushFileBuffers(SafeFileHandle fileHandle)
        {
            if (!FlushFileBuffers_Native(fileHandle))
                throw new Win32ExportException(string.Format("{0} failed: {1}", MethodBase.GetCurrentMethod().Name,
                    Marshal.GetLastWin32Error()));
        }

        [DllImport("Kernel32.dll", EntryPoint = "DeviceIoControl", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeviceIoControl_Native(IntPtr hDevice, uint controlCode, byte[] inBuffer,
            int inBufferSize, byte[] outBuffer, int outBufferSize, out int bytesReturned, IntPtr lpOverlapped);

        
        public static void DeviceIoControl(IntPtr handle, uint controlCode, byte[] inBuffer, int inBufferSize,
            byte[] outBuffer, int outBufferSize, out int bytesReturned)
        {
            if (!DeviceIoControl_Native(handle, controlCode, inBuffer, inBufferSize, outBuffer, outBufferSize,
                out bytesReturned, IntPtr.Zero))
                throw new Win32ExportException(string.Format("{0}: Control code {1:x} failed with error code {2:x}.",
                    MethodBase.GetCurrentMethod().Name, controlCode, Marshal.GetHRForLastWin32Error()));
        }

        [DllImport("Kernel32.dll", EntryPoint = "DeviceIoControl", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeviceIoControl_Native(IntPtr hDevice, uint controlCode, IntPtr inBuffer,
            int inBufferSize, IntPtr outBuffer, int outBufferSize, out int bytesReturned, IntPtr lpOverlapped);

        
        public static void DeviceIoControl(IntPtr handle, uint controlCode, IntPtr inBuffer, int inBufferSize,
            IntPtr outBuffer, int outBufferSize, out int bytesReturned)
        {
            if (!DeviceIoControl_Native(handle, controlCode, inBuffer, inBufferSize, outBuffer, outBufferSize,
                out bytesReturned, IntPtr.Zero))
                throw new Win32ExportException(string.Format("{0}: Control code {1:x} failed with error code {2:x}.",
                    MethodBase.GetCurrentMethod().Name, controlCode, Marshal.GetHRForLastWin32Error()));
        }

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcess")]
        private static extern IntPtr GetCurrentProcess_Native();

        public static IntPtr GetCurrentProcess()
        {
            var currentProcessNative = GetCurrentProcess_Native();
            var lastWin32Error = Marshal.GetLastWin32Error();
            if (currentProcessNative.ToInt32() != -1)
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, lastWin32Error));
            return currentProcessNative;
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupPrivilegeValue(string systemName, string name, out LUID luid);

        public static LUID LookupPrivilegeValue(string privilegeName)
        {
            var luid = new LUID();
            if (!LookupPrivilegeValue(null, privilegeName, out luid))
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, Marshal.GetLastWin32Error()));
            return luid;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
            [MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges, ref TOKEN_PRIVILEGES newState,
            uint bufferLengthInBytes, ref TOKEN_PRIVILEGES previousState, out uint returnLengthInBytes);

        public static TOKEN_PRIVILEGES AdjustTokenPrivileges(IntPtr tokenHandle, TOKEN_PRIVILEGES privileges)
        {
            var previousState = new TOKEN_PRIVILEGES();
            uint returnLengthInBytes = 0;
            if (!AdjustTokenPrivileges(tokenHandle, false, ref privileges, (uint) Marshal.SizeOf((object) privileges),
                ref previousState, out returnLengthInBytes))
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, Marshal.GetLastWin32Error()));
            return previousState;
        }

        [DllImport("advapi32.dll", EntryPoint = "OpenProcessToken", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken_Native(IntPtr processHandle, uint desiredAccess,
            out IntPtr tokenHandle);

        
        public static IntPtr OpenProcessToken(IntPtr processHandle, uint desiredAccess)
        {
            IntPtr tokenHandle;
            if (!OpenProcessToken_Native(processHandle, desiredAccess, out tokenHandle))
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, Marshal.GetLastWin32Error()));
            return tokenHandle;
        }

        [DllImport("advapi32.dll", EntryPoint = "RegLoadKey", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint
            RegLoadKey_Native(SafeRegistryHandle registryKey, string subKeyName, string fileName);

        public static void RegLoadKey(SafeRegistryHandle registryKey, string subKeyName, string fileName)
        {
            var num = RegLoadKey_Native(registryKey, subKeyName, fileName);
            if (num != ERROR_SUCCESS)
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, num));
        }

        [DllImport("advapi32.dll", EntryPoint = "RegUnLoadKey", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint RegUnLoadKey_Native(SafeRegistryHandle registryKey, string subKeyName);

        public static void RegUnloadKey(SafeRegistryHandle registryKey, string subKeyName)
        {
            var num = RegUnLoadKey_Native(registryKey, subKeyName);
            if (num != ERROR_SUCCESS)
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, num));
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint OROpenHive(string hivePath, out IntPtr rootKey);

        public static IntPtr OfflineRegistryOpenHive(string hivePath)
        {
            var rootKey = IntPtr.Zero;
            var num = OROpenHive(hivePath, out rootKey);
            if (num != ERROR_SUCCESS)
                throw new Win32ExportException(string.Format(
                    "{0}: This function failed with error 0x{1:x} for path: '{2}.", MethodBase.GetCurrentMethod().Name,
                    num, hivePath));
            return rootKey;
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint ORSaveHive(IntPtr hiveHandle, string hivePath, uint majorOsVersion,
            uint minorOSVersion);

        public static void OfflineRegistrySaveHive(IntPtr hiveHandle, string hivePath)
        {
            var num = ORSaveHive(hiveHandle, hivePath, 6U, 1U);
            if (num != ERROR_SUCCESS)
                throw new Win32ExportException(string.Format(
                    "{0}: This function failed with error 0x{1:x} for path: '{2}.", MethodBase.GetCurrentMethod().Name,
                    num, hivePath));
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint ORCloseHive(IntPtr rootKey);

        public static void OfflineRegistryCloseHive(IntPtr registryKey)
        {
            var num = ORCloseHive(registryKey);
            if (num != ERROR_SUCCESS)
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, num));
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint OROpenKey(IntPtr keyHandle, string subKeyName, out IntPtr subKeyHandle);

        public static IntPtr OfflineRegistryOpenSubKey(IntPtr keyHandle, string subKeyName)
        {
            var subKeyHandle = IntPtr.Zero;
            var num = OROpenKey(keyHandle, subKeyName, out subKeyHandle);
            if (num == ERROR_SUCCESS)
                return subKeyHandle;
            if (num == 2U)
                return IntPtr.Zero;
            throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                MethodBase.GetCurrentMethod().Name, num));
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint ORCloseKey(IntPtr keyHandle);

        public static void OfflineRegistryCloseSubKey(IntPtr keyHandle)
        {
            var num = ORCloseKey(keyHandle);
            if (num != ERROR_SUCCESS)
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, num));
        }

        [DllImport("offreg.dll", EntryPoint = "OREnumKey", CharSet = CharSet.Unicode)]
        private static extern uint OREnumKeySimple(IntPtr rootKey, uint index, StringBuilder subKeyName,
            ref uint subKeyCharacterCount, IntPtr subKeyClass, IntPtr subKeyClassCharacterCount, IntPtr fileTime);

        
        public static string OfflineRegistryEnumKey(IntPtr registryKey, uint index)
        {
            var subKeyName = new StringBuilder("keyName", (int) ImageConstants.RegistryKeyMaxNameSize);
            var registryKeyMaxNameSize = ImageConstants.RegistryKeyMaxNameSize;
            var num = OREnumKeySimple(registryKey, index, subKeyName, ref registryKeyMaxNameSize, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero);
            if (num == ERROR_SUCCESS)
                return subKeyName.ToString();
            if (num == ERROR_NO_MORE_ITEMS)
                return null;
            throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                MethodBase.GetCurrentMethod().Name, num));
        }

        [DllImport("offreg.dll", EntryPoint = "OREnumValue", CharSet = CharSet.Unicode)]
        private static extern uint OREnumValueSimple(IntPtr rootKey, uint index, StringBuilder valueName,
            ref uint valueCharacterCount, IntPtr valueType, IntPtr data, IntPtr dataSize);

        
        public static string OfflineRegistryEnumValue(IntPtr registryKey, uint index)
        {
            var valueName = new StringBuilder("valueName", (int) ImageConstants.RegistryValueMaxNameSize);
            var valueMaxNameSize = ImageConstants.RegistryValueMaxNameSize;
            var num = OREnumValueSimple(registryKey, index, valueName, ref valueMaxNameSize, IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero);
            if (num == ERROR_SUCCESS)
                return valueName.ToString();
            if (num == ERROR_NO_MORE_ITEMS)
                return null;
            throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                MethodBase.GetCurrentMethod().Name, num));
        }

        [DllImport("offreg.dll", EntryPoint = "ORGetValue", CharSet = CharSet.Unicode)]
        private static extern uint ORGetValueKind(IntPtr keyHandle, IntPtr subKey, string valueName, out uint valueType,
            IntPtr data, IntPtr dataLength);

        
        public static uint OfflineRegistryGetValueKind(IntPtr keyHandle, string valueName)
        {
            uint valueType = 0;
            var valueKind = ORGetValueKind(keyHandle, IntPtr.Zero, valueName, out valueType, IntPtr.Zero, IntPtr.Zero);
            if (valueKind != ERROR_SUCCESS)
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, valueKind));
            return valueType;
        }

        [DllImport("offreg.dll", EntryPoint = "ORGetValue", CharSet = CharSet.Unicode)]
        private static extern uint ORGetValueSize(IntPtr keyHandle, IntPtr subKey, string valueName, IntPtr valueType,
            IntPtr data, ref uint dataLength);

        
        public static uint OfflineRegistryGetValueSize(IntPtr keyHandle, string valueName)
        {
            uint dataLength = 0;
            var valueSize = ORGetValueSize(keyHandle, IntPtr.Zero, valueName, IntPtr.Zero, IntPtr.Zero, ref dataLength);
            if (valueSize != ERROR_SUCCESS)
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, valueSize));
            return dataLength;
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint ORGetValue(IntPtr keyHandle, IntPtr subKey, string valueName, out uint valueType,
            byte[] data, ref uint dataLength);

        public static object OfflineRegistryGetValue(IntPtr keyHandle, string valueName)
        {
            uint dataLength = 0;
            var valueSize = ORGetValueSize(keyHandle, IntPtr.Zero, valueName, IntPtr.Zero, IntPtr.Zero, ref dataLength);
            if (valueSize != ERROR_SUCCESS)
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, valueSize));
            var numArray = new byte[(int) dataLength];
            uint valueType;
            var num = ORGetValue(keyHandle, IntPtr.Zero, valueName, out valueType, numArray, ref dataLength);
            if (num != ERROR_SUCCESS)
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, num));
            switch (valueType)
            {
                case 1:
                    return Encoding.Unicode.GetString(numArray).Split(new char[1])[0];
                case 2:
                    return Environment.ExpandEnvironmentVariables(Encoding.Unicode.GetString(numArray))
                        .Split(new char[1])[0];
                case 3:
                    return numArray;
                case 4:
                    return (uint) ((numArray[3] << 24) | (numArray[2] << 16) | (numArray[1] << 8) | numArray[0]);
                case 5:
                    return (uint) ((numArray[0] << 24) | (numArray[1] << 16) | (numArray[2] << 8) | numArray[3]);
                case 6:
                    return Encoding.Unicode.GetString(numArray).Split(new char[1])[0];
                case 7:
                    var stringList1 = new List<string>(Encoding.Unicode.GetString(numArray).Split(new char[1]));
                    for (var index = 0; index < stringList1.Count; ++index)
                        if (string.IsNullOrEmpty(stringList1[index]))
                            stringList1.RemoveAt(index--);
                        else if (string.IsNullOrWhiteSpace(stringList1[index]))
                            stringList1.RemoveAt(index--);
                    return stringList1.ToArray();
                case 8:
                    var stringList2 = new List<string>(Encoding.Unicode.GetString(numArray).Split(new char[1]));
                    for (var index = 0; index < stringList2.Count; ++index)
                        if (string.IsNullOrEmpty(stringList2[index]))
                            stringList2.RemoveAt(index--);
                        else if (string.IsNullOrWhiteSpace(stringList2[index]))
                            stringList2.RemoveAt(index--);
                    return stringList2.ToArray();
                case 9:
                    return Encoding.Unicode.GetString(numArray).Split(new char[1])[0];
                case 10:
                    var stringList3 = new List<string>(Encoding.Unicode.GetString(numArray).Split(new char[1]));
                    for (var index = 0; index < stringList3.Count; ++index)
                        if (string.IsNullOrEmpty(stringList3[index]))
                            stringList3.RemoveAt(index--);
                        else if (string.IsNullOrWhiteSpace(stringList3[index]))
                            stringList3.RemoveAt(index--);
                    return stringList3.ToArray();
                default:
                    return numArray;
            }
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint ORSetValue(IntPtr keyHandle, string valueName, uint valueType, byte[] data,
            uint dataLength);

        
        public static void OfflineRegistrySetValue(IntPtr keyHandle, string valueName, uint valueType, byte[] data)
        {
            var keyHandle1 = keyHandle;
            var valueName1 = valueName;
            var num1 = (int) valueType;
            var data1 = data;
            var length = data1.Length;
            var num2 = ORSetValue(keyHandle1, valueName1, (uint) num1, data1, (uint) length);
            if (num2 != ERROR_SUCCESS)
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, num2));
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint ORCreateKey(IntPtr keyHandle, string subKeyName, string className, uint options,
            IntPtr securityDescriptor, out IntPtr newKeyHandle, out uint creationDisposition);

        
        public static IntPtr OfflineRegistryCreateKey(IntPtr keyHandle, string subKeyName)
        {
            var newKeyHandle = IntPtr.Zero;
            uint creationDisposition = 0;
            var key = ORCreateKey(keyHandle, subKeyName, null, 0U, IntPtr.Zero, out newKeyHandle,
                out creationDisposition);
            if (key != ERROR_SUCCESS)
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, key));
            if (creationDisposition != 1U)
                throw new ImageStorageException(string.Format("{0}: The key '{1}' already exists.",
                    MethodBase.GetCurrentMethod().Name, subKeyName));
            return newKeyHandle;
        }

        private struct FILETIME
        {
            public uint DateTimeLow;
            public uint DateTimeHigh;
        }

        public struct LUID
        {
             public uint LowPart;
            public int HighPart;
        }

        public struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
             public uint Attributes;
        }

        public struct TOKEN_PRIVILEGES
        {
             public uint PrivilegeCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public LUID_AND_ATTRIBUTES[] Privileges;
        }

        
        public struct DRIVE_LAYOUT_INFORMATION_MBR
        {
            public uint DiskSignature;
        }

        
        public struct DRIVE_LAYOUT_INFORMATION_GPT
        {
            public Guid DiskId;
            public ulong StartingUsableOffset;
            public ulong UsableLength;
            public uint MaxPartitionCount;
        }

        
        [StructLayout(LayoutKind.Explicit)]
        public struct DRIVE_LAYOUT_INFORMATION_UNION
        {
            [FieldOffset(0)] public DRIVE_LAYOUT_INFORMATION_MBR Mbr;
            [FieldOffset(0)] public DRIVE_LAYOUT_INFORMATION_GPT Gpt;
        }

        
        public struct PARTITION_INFORMATION_MBR
        {
            public byte PartitionType;
            [MarshalAs(UnmanagedType.Bool)] public bool BootIndicator;
            [MarshalAs(UnmanagedType.Bool)] public bool RecognizedPartition;
            public uint HiddenSectors;
        }

        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct PARTITION_INFORMATION_GPT
        {
            public Guid PartitionType;
            public Guid PartitionId;
            public ulong Attributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
            public string PartitionName;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct PARTITION_INFORMATION_UNION
        {
             [FieldOffset(0)] public PARTITION_INFORMATION_MBR Mbr;
             [FieldOffset(0)] public PARTITION_INFORMATION_GPT Gpt;
        }

        public struct PARTITION_INFORMATION_EX
        {
            public PARTITION_STYLE PartitionStyle;
            public long StartingOffset;
            public long PartitionLength;
            public int PartitionNumber;
            public bool RewritePartition;
            public PARTITION_INFORMATION_UNION DriveLayoutInformaiton;
        }

        
        public struct DRIVE_LAYOUT_INFORMATION_EX
        {
            public PARTITION_STYLE PartitionStyle;
            public int PartitionCount;
             public DRIVE_LAYOUT_INFORMATION_UNION DriveLayoutInformation;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.Struct)]
            public PARTITION_INFORMATION_EX[] PartitionEntry;
        }
    }
}