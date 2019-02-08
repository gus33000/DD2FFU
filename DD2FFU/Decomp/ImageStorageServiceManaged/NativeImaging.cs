// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.NativeImaging
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal sealed class NativeImaging
    {
        public enum LogLevel
        {
            levelError,
            levelWarning,
            levelInfo,
            levelDebug,
            levelInvalid
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "CreateImageStorageService",
            CallingConvention = CallingConvention.StdCall)]
        private static extern int CreateImageStorageServiceNative(out IntPtr serviceHandle,
            [MarshalAs(UnmanagedType.FunctionPtr)] LogFunction logError);

        public static IntPtr CreateImageStorageService(LogFunction logError)
        {
            var serviceHandle = IntPtr.Zero;
            var storageServiceNative = CreateImageStorageServiceNative(out serviceHandle, logError);
            if (Win32Exports.FAILED(storageServiceNative) || serviceHandle == IntPtr.Zero)
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    storageServiceNative));
            return serviceHandle;
        }

        [DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void CloseImageStorageService(IntPtr service);

        [DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetLoggingFunction(IntPtr service, LogLevel level,
            [MarshalAs(UnmanagedType.FunctionPtr)] LogFunction logFunction);

        [DllImport("ImageStorageService.dll", EntryPoint = "GetETWLogPath", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetETWLogPath_Native(IntPtr serviceHandle, StringBuilder logPath, uint pathLength);

        public static string GetETWLogPath(IntPtr serviceHandle)
        {
            var stringBuilder = new StringBuilder("etwLogPath", 1024);
            var serviceHandle1 = serviceHandle;
            var logPath = stringBuilder;
            var capacity = logPath.Capacity;
            var etwLogPathNative = GetETWLogPath_Native(serviceHandle1, logPath, (uint) capacity);
            if (Win32Exports.FAILED(etwLogPathNative))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name,
                    etwLogPathNative));
            return stringBuilder.ToString();
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "UpdateDiskLayout")]
        private static extern int UpdateDiskLayout_Native(IntPtr service, SafeFileHandle diskHandle);

        public static void UpdateDiskLayout(IntPtr service, SafeFileHandle diskHandle)
        {
            var hr = UpdateDiskLayout_Native(service, diskHandle);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                    MethodBase.GetCurrentMethod().Name, hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "InitializeVirtualHardDisk", CharSet = CharSet.Unicode)]
        private static extern int InitializeVirtualHardDisk_Native(IntPtr service, string fileName,
            [MarshalAs(UnmanagedType.Bool)] bool preparePartitions, ulong maxSizeInBytes,
            ref ImageStructures.STORE_ID storeId, uint partitionCount, uint sectorSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)]
            ImageStructures.PARTITION_ENTRY[] partitions, bool fAssignMountPoints, uint storeIdsCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 10)]
            ImageStructures.STORE_ID[] storeIds, out IntPtr storeHandle);

        public static IntPtr InitializeVirtualHardDisk(IntPtr service, string fileName, ulong maxSizeInBytes,
            ref ImageStructures.STORE_ID storeId, ImageStructures.PARTITION_ENTRY[] partitions, bool preparePartitions,
            bool fAssignMountPoints, uint sectorSize, ImageStructures.STORE_ID[] storeIds)
        {
            var storeHandle = IntPtr.Zero;
            var hr = InitializeVirtualHardDisk_Native(service, fileName, preparePartitions, maxSizeInBytes, ref storeId,
                (uint) partitions.Length, sectorSize, partitions, fAssignMountPoints, (uint) storeIds.Length, storeIds,
                out storeHandle);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                    MethodBase.GetCurrentMethod().Name, hr));
            return storeHandle;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "_NormalizeVolumeMountPoints@28", CharSet = CharSet.Unicode)]
        private static extern int NormalizeVolumeMountPoints_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            string mountPath);

        public static void NormalizeVolumeMountPoints(IntPtr service, ImageStructures.STORE_ID storeId,
            string mountPath)
        {
            var zero = IntPtr.Zero;
            var hr = NormalizeVolumeMountPoints_Native(service, storeId, mountPath);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                    MethodBase.GetCurrentMethod().Name, hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "WriteMountManagerRegistry2", CharSet = CharSet.Unicode)]
        private static extern int WriteMountManagerRegistry2_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            bool useWellKnownGuids);

        public static void WriteMountManagerRegistry2(IntPtr service, ImageStructures.STORE_ID storeId,
            bool useWellKnownGuids)
        {
            var zero = IntPtr.Zero;
            var hr = WriteMountManagerRegistry2_Native(service, storeId, useWellKnownGuids);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                    MethodBase.GetCurrentMethod().Name, hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "CreateEmptyVirtualDisk", CharSet = CharSet.Unicode)]
        private static extern int CreateEmptyVirtualDisk_Native(IntPtr service, string fileName,
            ref ImageStructures.STORE_ID storeId, ulong maxSizeInBytes, uint sectorSize, out IntPtr storeHandle);

        public static IntPtr CreateEmptyVirtualDisk(IntPtr service, string fileName,
            ref ImageStructures.STORE_ID storeId, ulong maxSizeInBytes, uint sectorSize)
        {
            var storeHandle = IntPtr.Zero;
            var virtualDiskNative = CreateEmptyVirtualDisk_Native(service, fileName, ref storeId, maxSizeInBytes,
                sectorSize, out storeHandle);
            if (Win32Exports.FAILED(virtualDiskNative))
                throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                    MethodBase.GetCurrentMethod().Name, virtualDiskNative));
            return storeHandle;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "OpenVirtualHardDisk", CharSet = CharSet.Unicode)]
        private static extern int OpenVirtualHardDisk_Native(IntPtr service, string fileName,
            [MarshalAs(UnmanagedType.Bool)] bool readOnly, out ImageStructures.STORE_ID storeId,
            out IntPtr storeHandle);

        public static IntPtr OpenVirtualHardDisk(IntPtr service, string fileName, out ImageStructures.STORE_ID storeId,
            bool readOnly)
        {
            var storeHandle = IntPtr.Zero;
            var hr = OpenVirtualHardDisk_Native(service, fileName, readOnly, out storeId, out storeHandle);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0}({1}) failed with error code: {2:x}",
                    MethodBase.GetCurrentMethod().Name, fileName, hr));
            return storeHandle;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "AttachToMountedImage", CharSet = CharSet.Unicode)]
        private static extern int AttachToMountedImage_Native(IntPtr service, string mountedPath,
            [MarshalAs(UnmanagedType.Bool)] bool openWithWriteAccess, StringBuilder imagePath,
            uint imagePathCharacterCount, out ImageStructures.STORE_ID storeId, out IntPtr storeHandle);

        public static void AttachToMountedImage(IntPtr service, string mountedDrivePath, bool readOnly,
            out string imagePath, out ImageStructures.STORE_ID storeId, out IntPtr storeHandle)
        {
            var stringBuilder = new StringBuilder(nameof(imagePath), 32768);
            var service1 = service;
            var mountedPath = mountedDrivePath;
            var num = !readOnly ? 1 : 0;
            var imagePath1 = stringBuilder;
            var capacity = imagePath1.Capacity;
            var mountedImageNative = AttachToMountedImage_Native(service1, mountedPath, num != 0, imagePath1,
                (uint) capacity, out storeId, out storeHandle);
            if (Win32Exports.FAILED(mountedImageNative))
                throw new ImageStorageException(string.Format("{0}({1}) failed with error code: {2:x}",
                    MethodBase.GetCurrentMethod().Name, mountedDrivePath, mountedImageNative));
            imagePath = stringBuilder.ToString();
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionPath", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionPath_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, StringBuilder path, uint pathSizeInCharacters);

        public static void GetPartitionPath(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, StringBuilder path, uint pathSizeInCharacters)
        {
            var partitionPathNative =
                GetPartitionPath_Native(serviceHandle, storeId, partitionName, path, pathSizeInCharacters);
            if (Win32Exports.FAILED(partitionPathNative))
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
                    MethodBase.GetCurrentMethod().Name, partitionName, partitionPathNative));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionPathNoContext", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionPathNoContext_Native(string partitionName, StringBuilder path,
            uint pathSizeInCharacters);

        public static void GetPartitionPathNoContext(string partitionName, StringBuilder path,
            uint pathSizeInCharacters)
        {
            var pathNoContextNative = GetPartitionPathNoContext_Native(partitionName, path, pathSizeInCharacters);
            if (Win32Exports.FAILED(pathNoContextNative))
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
                    MethodBase.GetCurrentMethod().Name, partitionName, pathNoContextNative));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionFileSystem", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionFileSystem_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, StringBuilder fileSystem, uint fileSystemSizeInCharacters);

        public static string GetPartitionFileSystem(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName)
        {
            var stringBuilder = new StringBuilder("fileSystem", 260);
            var serviceHandle1 = serviceHandle;
            var storeId1 = storeId;
            var partitionName1 = partitionName;
            var fileSystem = stringBuilder;
            var capacity = fileSystem.Capacity;
            var fileSystemNative =
                GetPartitionFileSystem_Native(serviceHandle1, storeId1, partitionName1, fileSystem, (uint) capacity);
            if (Win32Exports.FAILED(fileSystemNative))
                return "RAW";
                //throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
                //MethodBase.GetCurrentMethod().Name, partitionName, fileSystemNative));
            return stringBuilder.ToString();
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetDiskName", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetDiskName_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            StringBuilder fileSystem, uint fileSystemSizeInCharacters);

        public static string GetDiskName(IntPtr serviceHandle, ImageStructures.STORE_ID storeId)
        {
            var stringBuilder = new StringBuilder("diskName", 32768);
            var serviceHandle1 = serviceHandle;
            var storeId1 = storeId;
            var fileSystem = stringBuilder;
            var capacity = fileSystem.Capacity;
            var diskNameNative = GetDiskName_Native(serviceHandle1, storeId1, fileSystem, (uint) capacity);
            if (Win32Exports.FAILED(diskNameNative))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name,
                    diskNameNative));
            return stringBuilder.ToString();
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetVirtualHardDiskFileName", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetVhdFileName_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            StringBuilder imagePath, uint imagePathSizeInCharacters);

        public static string GetVhdFileName(IntPtr serviceHandle, ImageStructures.STORE_ID storeId)
        {
            var stringBuilder = new StringBuilder("imagePath", 32768);
            var serviceHandle1 = serviceHandle;
            var storeId1 = storeId;
            var imagePath = stringBuilder;
            var capacity = imagePath.Capacity;
            var vhdFileNameNative = GetVhdFileName_Native(serviceHandle1, storeId1, imagePath, (uint) capacity);
            if (Win32Exports.FAILED(vhdFileNameNative))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name,
                    vhdFileNameNative));
            return stringBuilder.ToString();
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetDiskId", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetDiskId_Native(IntPtr serviceHandle, SafeFileHandle diskHandle,
            out ImageStructures.STORE_ID storeId);

        public static ImageStructures.STORE_ID GetDiskId(IntPtr serviceHandle, SafeFileHandle diskHandle)
        {
            var storeId = new ImageStructures.STORE_ID();
            var diskIdNative = GetDiskId_Native(serviceHandle, diskHandle, out storeId);
            if (Win32Exports.FAILED(diskIdNative))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name,
                    diskIdNative));
            return storeId;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionType", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionType_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, out ImageStructures.PartitionType partitionType);

        public static ImageStructures.PartitionType GetPartitionType(IntPtr serviceHandle,
            ImageStructures.STORE_ID storeId, string partitionName)
        {
            var partitionType = new ImageStructures.PartitionType();
            var partitionTypeNative = GetPartitionType_Native(serviceHandle, storeId, partitionName, out partitionType);
            if (Win32Exports.FAILED(partitionTypeNative))
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
                    MethodBase.GetCurrentMethod().Name, partitionName, partitionTypeNative));
            return partitionType;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "SetPartitionType", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int SetPartitionType_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, ImageStructures.PartitionType partitionType);

        public static void SetPartitionType(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, ImageStructures.PartitionType partitionType)
        {
            var hr = SetPartitionType_Native(serviceHandle, storeId, partitionName, partitionType);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
                    MethodBase.GetCurrentMethod().Name, partitionName, hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "SetPartitionAttributes", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int SetPartitionAttributes_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, ImageStructures.PartitionAttributes attributes);

        public static void SetPartitionAttributes(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, ImageStructures.PartitionAttributes attributes)
        {
            var hr = SetPartitionAttributes_Native(serviceHandle, storeId, partitionName, attributes);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
                    MethodBase.GetCurrentMethod().Name, partitionName, hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionAttributes", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionAttributes_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, out ImageStructures.PartitionAttributes attributes);

        public static ImageStructures.PartitionAttributes GetPartitionAttributes(IntPtr serviceHandle,
            ImageStructures.STORE_ID storeId, string partitionName)
        {
            var attributes = new ImageStructures.PartitionAttributes();
            var attributesNative = GetPartitionAttributes_Native(serviceHandle, storeId, partitionName, out attributes);
            if (Win32Exports.FAILED(attributesNative))
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
                    MethodBase.GetCurrentMethod().Name, partitionName, attributesNative));
            return attributes;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "SetDiskAttributes", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int SetDiskAttributes_Native(IntPtr serviceHandle, IntPtr diskHandle,
            out ImageStructures.SetDiskAttributes attributes);

        public static void SetDiskAttributes(IntPtr serviceHandle, IntPtr diskHandle,
            ImageStructures.SetDiskAttributes attributes)
        {
            var attributes1 = attributes;
            var hr = SetDiskAttributes_Native(serviceHandle, diskHandle, out attributes1);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name,
                    hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "DismountVirtualHardDisk",
            CallingConvention = CallingConvention.StdCall)]
        private static extern int DismountVirtualHardDisk_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            [MarshalAs(UnmanagedType.Bool)] bool removeAccessPaths, [MarshalAs(UnmanagedType.Bool)] bool deleteFile,
            [MarshalAs(UnmanagedType.Bool)] bool fFailIfDiskMissing);

        public static void DismountVirtualHardDisk(IntPtr service, ImageStructures.STORE_ID storeId,
            bool removeAccessPaths, bool deleteFile, bool failIfDiskMissing = false)
        {
            var hr = DismountVirtualHardDisk_Native(service, storeId, removeAccessPaths, deleteFile, failIfDiskMissing);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "DismountVirtualHardDiskByFileName",
            CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern int DismountVirtualHardDiskByFileName_Native(IntPtr service, string fileName,
            [MarshalAs(UnmanagedType.Bool)] bool deleteFile);

        public static void DismountVirtualHardDiskByName(IntPtr service, string fileName, bool deleteFile)
        {
            var hr = DismountVirtualHardDiskByFileName_Native(service, fileName, deleteFile);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetSectorSize",
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetSectorSize_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            out uint bytesPerSector);

        public static uint GetSectorSize(IntPtr service, ImageStructures.STORE_ID storeId)
        {
            uint bytesPerSector = 0;
            var sectorSizeNative = GetSectorSize_Native(service, storeId, out bytesPerSector);
            if (Win32Exports.FAILED(sectorSizeNative))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    sectorSizeNative));
            return bytesPerSector;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionOffset", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionOffset_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            string partitionName, out ulong startingSector);

        public static ulong GetPartitionOffset(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName)
        {
            ulong startingSector = 0;
            var partitionOffsetNative = GetPartitionOffset_Native(service, storeId, partitionName, out startingSector);
            if (Win32Exports.FAILED(partitionOffsetNative))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    partitionOffsetNative));
            return startingSector;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionSize", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionSize_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            string partitionName, out ulong sectorCount);

        public static ulong GetPartitionSize(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName)
        {
            ulong sectorCount = 0;
            var partitionSizeNative = GetPartitionSize_Native(service, storeId, partitionName, out sectorCount);
            if (Win32Exports.FAILED(partitionSizeNative))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    partitionSizeNative));
            return sectorCount;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetFreeBytesOnVolume", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetFreeBytesOnVolume_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            string partitionName, out ulong freeBytes);

        public static ulong GetFreeBytesOnVolume(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName)
        {
            ulong freeBytes = 0;
            var bytesOnVolumeNative = GetFreeBytesOnVolume_Native(service, storeId, partitionName, out freeBytes);
            if (Win32Exports.FAILED(bytesOnVolumeNative))
                throw new ImageStorageException(string.Format("{0} failed for partition {1}: {2:x}.",
                    MethodBase.GetCurrentMethod().Name, partitionName, bytesOnVolumeNative));
            return freeBytes;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "OpenVolume", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int OpenVolumeHandle_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            string partitionName, uint requestedAccess, uint shareMode, out IntPtr volumeHandle);

        public static SafeFileHandle OpenVolumeHandle(IntPtr service, ImageStructures.STORE_ID storeId,
            string partitionName, FileAccess access, FileShare share)
        {
            var volumeHandle = IntPtr.Zero;
            uint shareMode = 0;
            uint requestedAccess = 0;
            if ((access & FileAccess.Read) != 0)
                requestedAccess |= 2147483648U;
            if ((access & FileAccess.Write) != 0)
                requestedAccess |= 1073741824U;
            if ((access & FileAccess.ReadWrite) != 0)
                requestedAccess = requestedAccess | 2147483648U | 1073741824U;
            if ((share & FileShare.Read) != FileShare.None)
                shareMode |= 1U;
            if ((share & FileShare.Write) != FileShare.None)
                shareMode |= 2U;
            if ((share & FileShare.ReadWrite) != FileShare.None)
                shareMode = shareMode | 1U | 2U;
            if ((share & FileShare.Delete) != FileShare.None)
                shareMode |= 4U;
            var hr = OpenVolumeHandle_Native(service, storeId, partitionName, requestedAccess, shareMode,
                out volumeHandle);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    hr));
            var safeFileHandle = new SafeFileHandle(volumeHandle, true);
            if (!safeFileHandle.IsInvalid)
                return safeFileHandle;
            throw new ImageStorageException(string.Format("{0} returned an invalid handle.",
                MethodBase.GetCurrentMethod().Name));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "CloseVolumeHandle",
            CallingConvention = CallingConvention.StdCall)]
        private static extern int CloseVolumeHandle_Native(IntPtr service, IntPtr volumeHandle);

        public static void CloseVolumeHandle(IntPtr service, IntPtr volumeHandle)
        {
            var hr = CloseVolumeHandle_Native(service, volumeHandle);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "LockAndDismountVolumeByHandle",
            CallingConvention = CallingConvention.StdCall)]
        private static extern int LockAndDismountVolumeByHandle_Native(IntPtr service, SafeFileHandle volumeHandle,
            [MarshalAs(UnmanagedType.Bool)] bool forceDismount);

        public static void LockAndDismountVolume(IntPtr service, SafeFileHandle volumeHandle, bool forceDismount)
        {
            var hr = LockAndDismountVolumeByHandle_Native(service, volumeHandle, forceDismount);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "UnlockVolumeByHandle",
            CallingConvention = CallingConvention.StdCall)]
        private static extern int UnlockVolumeByHandle_Native(IntPtr service, IntPtr volumeHandle);

        public static void UnlockVolume(IntPtr service, SafeHandle volumeHandle)
        {
            var hr = UnlockVolumeByHandle_Native(service, volumeHandle.DangerousGetHandle());
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "FormatPartition", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int FormatPartition_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            string partitionName, string fileSystem, uint cbClusterSize);

        public static void FormatPartition(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName,
            string fileSystem, uint cbClusterSize)
        {
            var hr = FormatPartition_Native(service, storeId, partitionName, fileSystem, cbClusterSize);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}.",
                    MethodBase.GetCurrentMethod().Name, partitionName, hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "AttachWOFToVolume", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AttachWOFToVolume_Native(IntPtr service, string volumePath);

        public static void AttachWOFToVolume(IntPtr service, string volumePath)
        {
            var volumeNative = AttachWOFToVolume_Native(service, volumePath);
            if (Win32Exports.FAILED(volumeNative))
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}.",
                    MethodBase.GetCurrentMethod().Name, volumePath, volumeNative));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "AddAccessPath", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AddAccessPath_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            string partitionName, string accessPath);

        public static void AddAccessPath(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName,
            string accessPath)
        {
            var hr = AddAccessPath_Native(service, storeId, partitionName, accessPath);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}.",
                    MethodBase.GetCurrentMethod().Name, partitionName, hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "WaitForVolumeArrival", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int WaitForVolumeArrival_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            string partitionName, int timeout);

        public static void WaitForVolumeArrival(IntPtr service, ImageStructures.STORE_ID storeId, string partitionName,
            int timeout)
        {
            var hr = WaitForVolumeArrival_Native(service, storeId, partitionName, timeout);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}.",
                    MethodBase.GetCurrentMethod().Name, partitionName, hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "ReadFromDisk", CharSet = CharSet.Unicode)]
        private static extern int ReadFromDisk_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            ulong diskOffset, uint byteCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            byte[] buffer);

        public static void ReadFromDisk(IntPtr service, ImageStructures.STORE_ID storeId, ulong diskOffset,
            byte[] buffer)
        {
            var hr = ReadFromDisk_Native(service, storeId, diskOffset, (uint) buffer.Length, buffer);
            if (Win32Exports.FAILED(hr))
                throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                    MethodBase.GetCurrentMethod().Name, hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionStyle", CharSet = CharSet.Unicode)]
        private static extern int GetPartitionStyle_Native(IntPtr service, SafeFileHandle hStore,
            out uint partitionStyle);

        public static uint GetPartitionStyle(IntPtr service, SafeFileHandle storeHandle)
        {
            uint partitionStyle = 0;
            var partitionStyleNative = GetPartitionStyle_Native(service, storeHandle, out partitionStyle);
            if (Win32Exports.FAILED(partitionStyleNative))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    partitionStyleNative));
            return partitionStyle;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetSectorCount", CharSet = CharSet.Unicode)]
        private static extern int GetSectorCount_Native(IntPtr service, SafeFileHandle hStore, out ulong sectorCount);

        public static ulong GetSectorCount(IntPtr service, SafeFileHandle storeHandle)
        {
            ulong sectorCount = 0;
            var sectorCountNative = GetSectorCount_Native(service, storeHandle, out sectorCount);
            if (Win32Exports.FAILED(sectorCountNative))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    sectorCountNative));
            return sectorCount;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetSectorSizeFromHandle", CharSet = CharSet.Unicode)]
        private static extern int GetSectorSizeFromHandle_Native(IntPtr service, SafeFileHandle hStore,
            out uint sectorCount);

        public static uint GetSectorSize(IntPtr service, SafeFileHandle storeHandle)
        {
            uint sectorCount = 0;
            var fromHandleNative = GetSectorSizeFromHandle_Native(service, storeHandle, out sectorCount);
            if (Win32Exports.FAILED(fromHandleNative))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    fromHandleNative));
            return sectorCount;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetBlockAllocationBitmap", CharSet = CharSet.Unicode)]
        private static extern int GetBlockAllocationBitmap_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            string partitionName, uint blockSize, byte[] blockBitmapBuffer, uint bitmapBufferSize);

        public static void GetBlockAllocationBitmap(IntPtr service, ImageStructures.STORE_ID storeId,
            string partitionName, uint blockSize, byte[] blockBitmapBuffer)
        {
            var service1 = service;
            var storeId1 = storeId;
            var partitionName1 = partitionName;
            var num = (int) blockSize;
            var blockBitmapBuffer1 = blockBitmapBuffer;
            var length = blockBitmapBuffer1.Length;
            var allocationBitmapNative = GetBlockAllocationBitmap_Native(service1, storeId1, partitionName1, (uint) num,
                blockBitmapBuffer1, (uint) length);
            if (Win32Exports.FAILED(allocationBitmapNative))
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    allocationBitmapNative));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "WaitForPartitions", CharSet = CharSet.Unicode)]
        private static extern int WaitForPartitions_Native(IntPtr service, ImageStructures.STORE_ID storeId,
            uint partitionCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
            ImageStructures.PARTITION_ENTRY[] partitions);

        public static IntPtr WaitForPartitions(IntPtr service, ImageStructures.STORE_ID storeId,
            ImageStructures.PARTITION_ENTRY[] partitions)
        {
            var zero = IntPtr.Zero;
            var hr = WaitForPartitions_Native(service, storeId, (uint) partitions.Length, partitions);
            if (!Win32Exports.FAILED(hr))
                return zero;
            throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                MethodBase.GetCurrentMethod().Name, hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "CreateUsnJournal", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int CreateUsnJournal_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName);

        public static void CreateUsnJournal(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName)
        {
            var usnJournalNative = CreateUsnJournal_Native(serviceHandle, storeId, partitionName);
            if (Win32Exports.FAILED(usnJournalNative))
                throw new ImageStorageException(string.Format("{0}({1} failed with error code: {2:x}",
                    MethodBase.GetCurrentMethod().Name, partitionName, usnJournalNative));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "CreateJunction", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int CreateJunction_Native(IntPtr serviceHandle, ImageStructures.STORE_ID storeId,
            string sourceName, string targetPartition, string targetPath,
            [MarshalAs(UnmanagedType.U1)] bool useWellKnownGuids);

        public static void CreateJunction(IntPtr serviceHandle, ImageStructures.STORE_ID storeId, string sourceName,
            string targetPartition, string targetName, bool useWellKnownGuids = false)
        {
            var junctionNative = CreateJunction_Native(serviceHandle, storeId, sourceName, targetPartition, targetName,
                useWellKnownGuids);
            if (Win32Exports.FAILED(junctionNative))
                throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                    MethodBase.GetCurrentMethod().Name, junctionNative));
        }
    }
}