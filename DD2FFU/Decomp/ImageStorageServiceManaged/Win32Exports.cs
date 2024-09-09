// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.Win32Exports
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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
        private static extern bool CloseHandle_Native(nint handle);

        public static void CloseHandle(nint handle)
        {
            if (!CloseHandle_Native(handle))
            {
                throw new Win32ExportException(string.Format("{0} failed with error {1}",
                                MethodBase.GetCurrentMethod().Name, Marshal.GetLastWin32Error()));
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "CreateFile", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern SafeFileHandle CreateFile_Native(string fileName, DesiredAccess desiredAccess,
            ShareMode shareMode, nint securityAttributes, CreationDisposition creationDisposition,
            FlagsAndAttributes flagsAndAttributes, nint templateFileHandle);


        public static SafeFileHandle CreateFile(string fileName, DesiredAccess desiredAccess, ShareMode shareMode,
            CreationDisposition creationDisposition, FlagsAndAttributes flagsAndAttributes)
        {
            SafeFileHandle fileNative = CreateFile_Native(fileName, desiredAccess, shareMode, nint.Zero, creationDisposition,
                flagsAndAttributes, nint.Zero);
            if (!fileNative.IsInvalid)
            {
                return fileNative;
            }

            int lastWin32Error = Marshal.GetLastWin32Error();
            throw new Win32ExportException(string.Format("{0}({1}) failed with error {2}",
                MethodBase.GetCurrentMethod().Name, string.IsNullOrEmpty(fileName) ? "" : fileName, lastWin32Error));
        }

        [DllImport("kernel32.dll", EntryPoint = "ReadFile", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReadFile_Native(SafeFileHandle fileHandle, out byte[] buffer, uint bytesToRead,
            out uint bytesRead, nint overlapped);


        public static void ReadFile(SafeFileHandle fileHandle, out byte[] buffer, uint bytesToRead, out uint bytesRead)
        {
            if (!ReadFile_Native(fileHandle, out buffer, bytesToRead, out bytesRead, nint.Zero))
            {
                throw new Win32ExportException(string.Format("ReadFile failed with error: {0}",
                                Marshal.GetLastWin32Error()));
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "ReadFile", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReadFile_Native(SafeFileHandle fileHandle, nint buffer, uint bytesToRead,
            out uint bytesRead, nint overlapped);


        public static void ReadFile(SafeFileHandle fileHandle, nint buffer, uint bytesToRead, out uint bytesRead)
        {
            if (!ReadFile_Native(fileHandle, buffer, bytesToRead, out bytesRead, nint.Zero))
            {
                throw new Win32ExportException(string.Format("ReadFile failed with error: {0}",
                                Marshal.GetLastWin32Error()));
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "WriteFile", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WriteFile_Native(SafeFileHandle handle, nint buffer, uint numBytesToWrite,
            out uint numBytesWritten, nint overlapped);


        public static void WriteFile(SafeFileHandle fileHandle, nint buffer, uint bytesToWrite, out uint bytesWritten)
        {
            if (!WriteFile_Native(fileHandle, buffer, bytesToWrite, out bytesWritten, nint.Zero))
            {
                throw new Win32ExportException(string.Format("WriteFile failed with error: {0}",
                                Marshal.GetLastWin32Error()));
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "SetFilePointerEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetFilePointerEx_Native(SafeFileHandle fileHandle, long distanceToMove,
            out long newFilePointer, MoveMethod moveMethod);


        public static void SetFilePointerEx(SafeFileHandle fileHandle, long distanceToMove, out long newFileLocation,
            MoveMethod moveMethod)
        {
            if (!SetFilePointerEx_Native(fileHandle, distanceToMove, out newFileLocation, moveMethod))
            {
                throw new Win32ExportException(string.Format("SetFilePointerEx failed with error: {0}",
                                Marshal.GetLastWin32Error()));
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "VirtualAlloc", SetLastError = true)]
        private static extern nint VirtualAlloc_Native(nint lpAddress, nuint sizeInBytes,
            AllocationType allocationType, MemoryProtection memoryProtection);


        public static nint VirtualAlloc(nuint sizeInBytes, AllocationType allocationType,
            MemoryProtection memoryProtection)
        {
            nint num = VirtualAlloc_Native(nint.Zero, sizeInBytes, allocationType, memoryProtection);
            return !(num == nint.Zero)
                ? num
                : throw new Win32ExportException(string.Format("{0} failed with error {1}",
                MethodBase.GetCurrentMethod().Name, Marshal.GetLastWin32Error()));
        }

        [DllImport("kernel32.dll", EntryPoint = "VirtualFree", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool VirtualFree_Native(nint address, nuint sizeInBytes, FreeType freeType);


        public static void VirtualFree(nint address, FreeType freeType)
        {
            nuint zero = nuint.Zero;
            if (!VirtualFree_Native(address, zero, freeType))
            {
                throw new Win32ExportException(string.Format("{0} failed with error {1}",
                                MethodBase.GetCurrentMethod().Name, Marshal.GetLastWin32Error()));
            }
        }


        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int memcmp(byte[] buffer1, nint buffer2, nuint count);

        [DllImport("kernel32.dll", EntryPoint = "FlushFileBuffers", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlushFileBuffers_Native(SafeFileHandle fileHandle);


        public static void FlushFileBuffers(SafeFileHandle fileHandle)
        {
            if (!FlushFileBuffers_Native(fileHandle))
            {
                throw new Win32ExportException(string.Format("{0} failed: {1}", MethodBase.GetCurrentMethod().Name,
                                Marshal.GetLastWin32Error()));
            }
        }

        [DllImport("Kernel32.dll", EntryPoint = "DeviceIoControl", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeviceIoControl_Native(nint hDevice, uint controlCode, byte[] inBuffer,
            int inBufferSize, byte[] outBuffer, int outBufferSize, out int bytesReturned, nint lpOverlapped);


        public static void DeviceIoControl(nint handle, uint controlCode, byte[] inBuffer, int inBufferSize,
            byte[] outBuffer, int outBufferSize, out int bytesReturned)
        {
            if (!DeviceIoControl_Native(handle, controlCode, inBuffer, inBufferSize, outBuffer, outBufferSize,
                out bytesReturned, nint.Zero))
            {
                throw new Win32ExportException(string.Format("{0}: Control code {1:x} failed with error code {2:x}.",
                                MethodBase.GetCurrentMethod().Name, controlCode, Marshal.GetHRForLastWin32Error()));
            }
        }

        [DllImport("Kernel32.dll", EntryPoint = "DeviceIoControl", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeviceIoControl_Native(nint hDevice, uint controlCode, nint inBuffer,
            int inBufferSize, nint outBuffer, int outBufferSize, out int bytesReturned, nint lpOverlapped);


        public static void DeviceIoControl(nint handle, uint controlCode, nint inBuffer, int inBufferSize,
            nint outBuffer, int outBufferSize, out int bytesReturned)
        {
            if (!DeviceIoControl_Native(handle, controlCode, inBuffer, inBufferSize, outBuffer, outBufferSize,
                out bytesReturned, nint.Zero))
            {
                throw new Win32ExportException(string.Format("{0}: Control code {1:x} failed with error code {2:x}.",
                                MethodBase.GetCurrentMethod().Name, controlCode, Marshal.GetHRForLastWin32Error()));
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcess")]
        private static extern nint GetCurrentProcess_Native();

        public static nint GetCurrentProcess()
        {
            nint currentProcessNative = GetCurrentProcess_Native();
            int lastWin32Error = Marshal.GetLastWin32Error();
            return currentProcessNative.ToInt32() != -1
                ? throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, lastWin32Error))
                : currentProcessNative;
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupPrivilegeValue(string systemName, string name, out LUID luid);

        public static LUID LookupPrivilegeValue(string privilegeName)
        {
            _ = new LUID();

            return !LookupPrivilegeValue(null, privilegeName, out LUID luid)
                ? throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, Marshal.GetLastWin32Error()))
                : luid;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges(nint TokenHandle,
            [MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges, ref TOKEN_PRIVILEGES newState,
            uint bufferLengthInBytes, ref TOKEN_PRIVILEGES previousState, out uint returnLengthInBytes);

        public static TOKEN_PRIVILEGES AdjustTokenPrivileges(nint tokenHandle, TOKEN_PRIVILEGES privileges)
        {
            TOKEN_PRIVILEGES previousState = new();

            return !AdjustTokenPrivileges(tokenHandle, false, ref privileges, (uint)Marshal.SizeOf((object)privileges),
                ref previousState, out _)
                ? throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, Marshal.GetLastWin32Error()))
                : previousState;
        }

        [DllImport("advapi32.dll", EntryPoint = "OpenProcessToken", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken_Native(nint processHandle, uint desiredAccess,
            out nint tokenHandle);


        public static nint OpenProcessToken(nint processHandle, uint desiredAccess)
        {
            return !OpenProcessToken_Native(processHandle, desiredAccess, out nint tokenHandle)
                ? throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, Marshal.GetLastWin32Error()))
                : tokenHandle;
        }

        [DllImport("advapi32.dll", EntryPoint = "RegLoadKey", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint
            RegLoadKey_Native(SafeRegistryHandle registryKey, string subKeyName, string fileName);

        public static void RegLoadKey(SafeRegistryHandle registryKey, string subKeyName, string fileName)
        {
            uint num = RegLoadKey_Native(registryKey, subKeyName, fileName);
            if (num != ERROR_SUCCESS)
            {
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                                MethodBase.GetCurrentMethod().Name, num));
            }
        }

        [DllImport("advapi32.dll", EntryPoint = "RegUnLoadKey", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint RegUnLoadKey_Native(SafeRegistryHandle registryKey, string subKeyName);

        public static void RegUnloadKey(SafeRegistryHandle registryKey, string subKeyName)
        {
            uint num = RegUnLoadKey_Native(registryKey, subKeyName);
            if (num != ERROR_SUCCESS)
            {
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                                MethodBase.GetCurrentMethod().Name, num));
            }
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint OROpenHive(string hivePath, out nint rootKey);

        public static nint OfflineRegistryOpenHive(string hivePath)
        {
            uint num = OROpenHive(hivePath, out nint rootKey);
            return num != ERROR_SUCCESS
                ? throw new Win32ExportException(string.Format(
                    "{0}: This function failed with error 0x{1:x} for path: '{2}.", MethodBase.GetCurrentMethod().Name,
                    num, hivePath))
                : rootKey;
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint ORSaveHive(nint hiveHandle, string hivePath, uint majorOsVersion,
            uint minorOSVersion);

        public static void OfflineRegistrySaveHive(nint hiveHandle, string hivePath)
        {
            uint num = ORSaveHive(hiveHandle, hivePath, 6U, 1U);
            if (num != ERROR_SUCCESS)
            {
                throw new Win32ExportException(string.Format(
                                "{0}: This function failed with error 0x{1:x} for path: '{2}.", MethodBase.GetCurrentMethod().Name,
                                num, hivePath));
            }
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint ORCloseHive(nint rootKey);

        public static void OfflineRegistryCloseHive(nint registryKey)
        {
            uint num = ORCloseHive(registryKey);
            if (num != ERROR_SUCCESS)
            {
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                                MethodBase.GetCurrentMethod().Name, num));
            }
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint OROpenKey(nint keyHandle, string subKeyName, out nint subKeyHandle);

        public static nint OfflineRegistryOpenSubKey(nint keyHandle, string subKeyName)
        {
            uint num = OROpenKey(keyHandle, subKeyName, out nint subKeyHandle);
            return num == ERROR_SUCCESS
                ? subKeyHandle
                : num == 2U
                            ? nint.Zero
                            : throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                            MethodBase.GetCurrentMethod().Name, num));
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint ORCloseKey(nint keyHandle);

        public static void OfflineRegistryCloseSubKey(nint keyHandle)
        {
            uint num = ORCloseKey(keyHandle);
            if (num != ERROR_SUCCESS)
            {
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                                MethodBase.GetCurrentMethod().Name, num));
            }
        }

        [DllImport("offreg.dll", EntryPoint = "OREnumKey", CharSet = CharSet.Unicode)]
        private static extern uint OREnumKeySimple(nint rootKey, uint index, StringBuilder subKeyName,
            ref uint subKeyCharacterCount, nint subKeyClass, nint subKeyClassCharacterCount, nint fileTime);


        public static string OfflineRegistryEnumKey(nint registryKey, uint index)
        {
            StringBuilder subKeyName = new("keyName", (int)ImageConstants.RegistryKeyMaxNameSize);
            uint registryKeyMaxNameSize = ImageConstants.RegistryKeyMaxNameSize;
            uint num = OREnumKeySimple(registryKey, index, subKeyName, ref registryKeyMaxNameSize, nint.Zero,
                nint.Zero, nint.Zero);
            return num == ERROR_SUCCESS
                ? subKeyName.ToString()
                : num == ERROR_NO_MORE_ITEMS
                            ? null
                            : throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                            MethodBase.GetCurrentMethod().Name, num));
        }

        [DllImport("offreg.dll", EntryPoint = "OREnumValue", CharSet = CharSet.Unicode)]
        private static extern uint OREnumValueSimple(nint rootKey, uint index, StringBuilder valueName,
            ref uint valueCharacterCount, nint valueType, nint data, nint dataSize);


        public static string OfflineRegistryEnumValue(nint registryKey, uint index)
        {
            StringBuilder valueName = new("valueName", (int)ImageConstants.RegistryValueMaxNameSize);
            uint valueMaxNameSize = ImageConstants.RegistryValueMaxNameSize;
            uint num = OREnumValueSimple(registryKey, index, valueName, ref valueMaxNameSize, nint.Zero, nint.Zero,
                nint.Zero);
            return num == ERROR_SUCCESS
                ? valueName.ToString()
                : num == ERROR_NO_MORE_ITEMS
                            ? null
                            : throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                            MethodBase.GetCurrentMethod().Name, num));
        }

        [DllImport("offreg.dll", EntryPoint = "ORGetValue", CharSet = CharSet.Unicode)]
        private static extern uint ORGetValueKind(nint keyHandle, nint subKey, string valueName, out uint valueType,
            nint data, nint dataLength);


        public static uint OfflineRegistryGetValueKind(nint keyHandle, string valueName)
        {
            uint valueKind = ORGetValueKind(keyHandle, nint.Zero, valueName, out uint valueType, nint.Zero, nint.Zero);
            return valueKind != ERROR_SUCCESS
                ? throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, valueKind))
                : valueType;
        }

        [DllImport("offreg.dll", EntryPoint = "ORGetValue", CharSet = CharSet.Unicode)]
        private static extern uint ORGetValueSize(nint keyHandle, nint subKey, string valueName, nint valueType,
            nint data, ref uint dataLength);


        public static uint OfflineRegistryGetValueSize(nint keyHandle, string valueName)
        {
            uint dataLength = 0;
            uint valueSize = ORGetValueSize(keyHandle, nint.Zero, valueName, nint.Zero, nint.Zero, ref dataLength);
            return valueSize != ERROR_SUCCESS
                ? throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                    MethodBase.GetCurrentMethod().Name, valueSize))
                : dataLength;
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint ORGetValue(nint keyHandle, nint subKey, string valueName, out uint valueType,
            byte[] data, ref uint dataLength);

        public static object OfflineRegistryGetValue(nint keyHandle, string valueName)
        {
            uint dataLength = 0;
            uint valueSize = ORGetValueSize(keyHandle, nint.Zero, valueName, nint.Zero, nint.Zero, ref dataLength);
            if (valueSize != ERROR_SUCCESS)
            {
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                                MethodBase.GetCurrentMethod().Name, valueSize));
            }

            byte[] numArray = new byte[(int)dataLength];
            uint num = ORGetValue(keyHandle, nint.Zero, valueName, out uint valueType, numArray, ref dataLength);
            if (num != ERROR_SUCCESS)
            {
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                                MethodBase.GetCurrentMethod().Name, num));
            }

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
                    return (uint)((numArray[3] << 24) | (numArray[2] << 16) | (numArray[1] << 8) | numArray[0]);
                case 5:
                    return (uint)((numArray[0] << 24) | (numArray[1] << 16) | (numArray[2] << 8) | numArray[3]);
                case 6:
                    return Encoding.Unicode.GetString(numArray).Split(new char[1])[0];
                case 7:
                    List<string> stringList1 = new(Encoding.Unicode.GetString(numArray).Split(new char[1]));
                    for (int index = 0; index < stringList1.Count; ++index)
                    {
                        if (string.IsNullOrEmpty(stringList1[index]))
                        {
                            stringList1.RemoveAt(index--);
                        }
                        else if (string.IsNullOrWhiteSpace(stringList1[index]))
                        {
                            stringList1.RemoveAt(index--);
                        }
                    }

                    return stringList1.ToArray();
                case 8:
                    List<string> stringList2 = new(Encoding.Unicode.GetString(numArray).Split(new char[1]));
                    for (int index = 0; index < stringList2.Count; ++index)
                    {
                        if (string.IsNullOrEmpty(stringList2[index]))
                        {
                            stringList2.RemoveAt(index--);
                        }
                        else if (string.IsNullOrWhiteSpace(stringList2[index]))
                        {
                            stringList2.RemoveAt(index--);
                        }
                    }

                    return stringList2.ToArray();
                case 9:
                    return Encoding.Unicode.GetString(numArray).Split(new char[1])[0];
                case 10:
                    List<string> stringList3 = new(Encoding.Unicode.GetString(numArray).Split(new char[1]));
                    for (int index = 0; index < stringList3.Count; ++index)
                    {
                        if (string.IsNullOrEmpty(stringList3[index]))
                        {
                            stringList3.RemoveAt(index--);
                        }
                        else if (string.IsNullOrWhiteSpace(stringList3[index]))
                        {
                            stringList3.RemoveAt(index--);
                        }
                    }

                    return stringList3.ToArray();
                default:
                    return numArray;
            }
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint ORSetValue(nint keyHandle, string valueName, uint valueType, byte[] data,
            uint dataLength);


        public static void OfflineRegistrySetValue(nint keyHandle, string valueName, uint valueType, byte[] data)
        {
            nint keyHandle1 = keyHandle;
            string valueName1 = valueName;
            int num1 = (int)valueType;
            byte[] data1 = data;
            int length = data1.Length;
            uint num2 = ORSetValue(keyHandle1, valueName1, (uint)num1, data1, (uint)length);
            if (num2 != ERROR_SUCCESS)
            {
                throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                                MethodBase.GetCurrentMethod().Name, num2));
            }
        }

        [DllImport("offreg.dll", CharSet = CharSet.Unicode)]
        private static extern uint ORCreateKey(nint keyHandle, string subKeyName, string className, uint options,
            nint securityDescriptor, out nint newKeyHandle, out uint creationDisposition);


        public static nint OfflineRegistryCreateKey(nint keyHandle, string subKeyName)
        {
            uint key = ORCreateKey(keyHandle, subKeyName, null, 0U, nint.Zero, out nint newKeyHandle,
                out uint creationDisposition);
            return key != ERROR_SUCCESS
                ? throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.",
                                MethodBase.GetCurrentMethod().Name, key))
                : creationDisposition != 1U
                            ? throw new ImageStorageException(string.Format("{0}: The key '{1}' already exists.",
                                MethodBase.GetCurrentMethod().Name, subKeyName))
                            : newKeyHandle;
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