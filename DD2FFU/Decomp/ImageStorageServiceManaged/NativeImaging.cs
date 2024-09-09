// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.NativeImaging
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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
        private static extern int CreateImageStorageServiceNative(out nint serviceHandle,
            [MarshalAs(UnmanagedType.FunctionPtr)] LogFunction logError);

        public static nint CreateImageStorageService(LogFunction logError)
        {
            int storageServiceNative = CreateImageStorageServiceNative(out nint serviceHandle, logError);
            return Win32Exports.FAILED(storageServiceNative) || serviceHandle == nint.Zero
                ? throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    storageServiceNative))
                : serviceHandle;
        }

        [DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void CloseImageStorageService(nint service);

        [DllImport("ImageStorageService.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetLoggingFunction(nint service, LogLevel level,
            [MarshalAs(UnmanagedType.FunctionPtr)] LogFunction logFunction);

        [DllImport("ImageStorageService.dll", EntryPoint = "GetETWLogPath", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetETWLogPath_Native(nint serviceHandle, StringBuilder logPath, uint pathLength);

        public static string GetETWLogPath(nint serviceHandle)
        {
            StringBuilder stringBuilder = new("etwLogPath", 1024);
            nint serviceHandle1 = serviceHandle;
            StringBuilder logPath = stringBuilder;
            int capacity = logPath.Capacity;
            int etwLogPathNative = GetETWLogPath_Native(serviceHandle1, logPath, (uint)capacity);
            return Win32Exports.FAILED(etwLogPathNative)
                ? throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name,
                    etwLogPathNative))
                : stringBuilder.ToString();
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "UpdateDiskLayout")]
        private static extern int UpdateDiskLayout_Native(nint service, SafeFileHandle diskHandle);

        public static void UpdateDiskLayout(nint service, SafeFileHandle diskHandle)
        {
            int hr = UpdateDiskLayout_Native(service, diskHandle);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                                MethodBase.GetCurrentMethod().Name, hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "InitializeVirtualHardDisk", CharSet = CharSet.Unicode)]
        private static extern int InitializeVirtualHardDisk_Native(nint service, string fileName,
            [MarshalAs(UnmanagedType.Bool)] bool preparePartitions, ulong maxSizeInBytes,
            ref ImageStructures.STORE_ID storeId, uint partitionCount, uint sectorSize,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)]
            ImageStructures.PARTITION_ENTRY[] partitions, bool fAssignMountPoints, uint storeIdsCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 10)]
            ImageStructures.STORE_ID[] storeIds, out nint storeHandle);

        public static nint InitializeVirtualHardDisk(nint service, string fileName, ulong maxSizeInBytes,
            ref ImageStructures.STORE_ID storeId, ImageStructures.PARTITION_ENTRY[] partitions, bool preparePartitions,
            bool fAssignMountPoints, uint sectorSize, ImageStructures.STORE_ID[] storeIds)
        {
            int hr = InitializeVirtualHardDisk_Native(service, fileName, preparePartitions, maxSizeInBytes, ref storeId,
                (uint)partitions.Length, sectorSize, partitions, fAssignMountPoints, (uint)storeIds.Length, storeIds,
                out nint storeHandle);
            return Win32Exports.FAILED(hr)
                ? throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                    MethodBase.GetCurrentMethod().Name, hr))
                : storeHandle;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "_NormalizeVolumeMountPoints@28", CharSet = CharSet.Unicode)]
        private static extern int NormalizeVolumeMountPoints_Native(nint service, ImageStructures.STORE_ID storeId,
            string mountPath);

        public static void NormalizeVolumeMountPoints(nint service, ImageStructures.STORE_ID storeId,
            string mountPath)
        {
            int hr = NormalizeVolumeMountPoints_Native(service, storeId, mountPath);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                                MethodBase.GetCurrentMethod().Name, hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "WriteMountManagerRegistry2", CharSet = CharSet.Unicode)]
        private static extern int WriteMountManagerRegistry2_Native(nint service, ImageStructures.STORE_ID storeId,
            bool useWellKnownGuids);

        public static void WriteMountManagerRegistry2(nint service, ImageStructures.STORE_ID storeId,
            bool useWellKnownGuids)
        {
            int hr = WriteMountManagerRegistry2_Native(service, storeId, useWellKnownGuids);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                                MethodBase.GetCurrentMethod().Name, hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "CreateEmptyVirtualDisk", CharSet = CharSet.Unicode)]
        private static extern int CreateEmptyVirtualDisk_Native(nint service, string fileName,
            ref ImageStructures.STORE_ID storeId, ulong maxSizeInBytes, uint sectorSize, out nint storeHandle);

        public static nint CreateEmptyVirtualDisk(nint service, string fileName,
            ref ImageStructures.STORE_ID storeId, ulong maxSizeInBytes, uint sectorSize)
        {
            int virtualDiskNative = CreateEmptyVirtualDisk_Native(service, fileName, ref storeId, maxSizeInBytes,
                sectorSize, out nint storeHandle);
            return Win32Exports.FAILED(virtualDiskNative)
                ? throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                    MethodBase.GetCurrentMethod().Name, virtualDiskNative))
                : storeHandle;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "OpenVirtualHardDisk", CharSet = CharSet.Unicode)]
        private static extern int OpenVirtualHardDisk_Native(nint service, string fileName,
            [MarshalAs(UnmanagedType.Bool)] bool readOnly, out ImageStructures.STORE_ID storeId,
            out nint storeHandle);

        public static nint OpenVirtualHardDisk(nint service, string fileName, out ImageStructures.STORE_ID storeId,
            bool readOnly)
        {
            int hr = OpenVirtualHardDisk_Native(service, fileName, readOnly, out storeId, out nint storeHandle);
            return Win32Exports.FAILED(hr)
                ? throw new ImageStorageException(string.Format("{0}({1}) failed with error code: {2:x}",
                    MethodBase.GetCurrentMethod().Name, fileName, hr))
                : storeHandle;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "AttachToMountedImage", CharSet = CharSet.Unicode)]
        private static extern int AttachToMountedImage_Native(nint service, string mountedPath,
            [MarshalAs(UnmanagedType.Bool)] bool openWithWriteAccess, StringBuilder imagePath,
            uint imagePathCharacterCount, out ImageStructures.STORE_ID storeId, out nint storeHandle);

        public static void AttachToMountedImage(nint service, string mountedDrivePath, bool readOnly,
            out string imagePath, out ImageStructures.STORE_ID storeId, out nint storeHandle)
        {
            StringBuilder stringBuilder = new(nameof(imagePath), 32768);
            nint service1 = service;
            string mountedPath = mountedDrivePath;
            int num = !readOnly ? 1 : 0;
            StringBuilder imagePath1 = stringBuilder;
            int capacity = imagePath1.Capacity;
            int mountedImageNative = AttachToMountedImage_Native(service1, mountedPath, num != 0, imagePath1,
                (uint)capacity, out storeId, out storeHandle);
            if (Win32Exports.FAILED(mountedImageNative))
            {
                throw new ImageStorageException(string.Format("{0}({1}) failed with error code: {2:x}",
                                MethodBase.GetCurrentMethod().Name, mountedDrivePath, mountedImageNative));
            }

            imagePath = stringBuilder.ToString();
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionPath", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionPath_Native(nint serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, StringBuilder path, uint pathSizeInCharacters);

        public static void GetPartitionPath(nint serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, StringBuilder path, uint pathSizeInCharacters)
        {
            int partitionPathNative =
                GetPartitionPath_Native(serviceHandle, storeId, partitionName, path, pathSizeInCharacters);
            if (Win32Exports.FAILED(partitionPathNative))
            {
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
                                MethodBase.GetCurrentMethod().Name, partitionName, partitionPathNative));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionPathNoContext", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionPathNoContext_Native(string partitionName, StringBuilder path,
            uint pathSizeInCharacters);

        public static void GetPartitionPathNoContext(string partitionName, StringBuilder path,
            uint pathSizeInCharacters)
        {
            int pathNoContextNative = GetPartitionPathNoContext_Native(partitionName, path, pathSizeInCharacters);
            if (Win32Exports.FAILED(pathNoContextNative))
            {
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
                                MethodBase.GetCurrentMethod().Name, partitionName, pathNoContextNative));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionFileSystem", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionFileSystem_Native(nint serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, StringBuilder fileSystem, uint fileSystemSizeInCharacters);

        public static string GetPartitionFileSystem(nint serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName)
        {
            StringBuilder stringBuilder = new("fileSystem", 260);
            nint serviceHandle1 = serviceHandle;
            ImageStructures.STORE_ID storeId1 = storeId;
            string partitionName1 = partitionName;
            StringBuilder fileSystem = stringBuilder;
            int capacity = fileSystem.Capacity;
            int fileSystemNative =
                GetPartitionFileSystem_Native(serviceHandle1, storeId1, partitionName1, fileSystem, (uint)capacity);
            if (Win32Exports.FAILED(fileSystemNative))
            {
                return "RAW";
            }
            //throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
            //MethodBase.GetCurrentMethod().Name, partitionName, fileSystemNative));
            return stringBuilder.ToString();
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetDiskName", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetDiskName_Native(nint serviceHandle, ImageStructures.STORE_ID storeId,
            StringBuilder fileSystem, uint fileSystemSizeInCharacters);

        public static string GetDiskName(nint serviceHandle, ImageStructures.STORE_ID storeId)
        {
            StringBuilder stringBuilder = new("diskName", 32768);
            nint serviceHandle1 = serviceHandle;
            ImageStructures.STORE_ID storeId1 = storeId;
            StringBuilder fileSystem = stringBuilder;
            int capacity = fileSystem.Capacity;
            int diskNameNative = GetDiskName_Native(serviceHandle1, storeId1, fileSystem, (uint)capacity);
            return Win32Exports.FAILED(diskNameNative)
                ? throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name,
                    diskNameNative))
                : stringBuilder.ToString();
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetVirtualHardDiskFileName", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetVhdFileName_Native(nint serviceHandle, ImageStructures.STORE_ID storeId,
            StringBuilder imagePath, uint imagePathSizeInCharacters);

        public static string GetVhdFileName(nint serviceHandle, ImageStructures.STORE_ID storeId)
        {
            StringBuilder stringBuilder = new("imagePath", 32768);
            nint serviceHandle1 = serviceHandle;
            ImageStructures.STORE_ID storeId1 = storeId;
            StringBuilder imagePath = stringBuilder;
            int capacity = imagePath.Capacity;
            int vhdFileNameNative = GetVhdFileName_Native(serviceHandle1, storeId1, imagePath, (uint)capacity);
            return Win32Exports.FAILED(vhdFileNameNative)
                ? throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name,
                    vhdFileNameNative))
                : stringBuilder.ToString();
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetDiskId", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetDiskId_Native(nint serviceHandle, SafeFileHandle diskHandle,
            out ImageStructures.STORE_ID storeId);

        public static ImageStructures.STORE_ID GetDiskId(nint serviceHandle, SafeFileHandle diskHandle)
        {
            _ = new ImageStructures.STORE_ID();

            int diskIdNative = GetDiskId_Native(serviceHandle, diskHandle, out ImageStructures.STORE_ID storeId);
            return Win32Exports.FAILED(diskIdNative)
                ? throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name,
                    diskIdNative))
                : storeId;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionType", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionType_Native(nint serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, out ImageStructures.PartitionType partitionType);

        public static ImageStructures.PartitionType GetPartitionType(nint serviceHandle,
            ImageStructures.STORE_ID storeId, string partitionName)
        {
            _ = new ImageStructures.PartitionType();

            int partitionTypeNative = GetPartitionType_Native(serviceHandle, storeId, partitionName, out ImageStructures.PartitionType partitionType);
            return Win32Exports.FAILED(partitionTypeNative)
                ? throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
                    MethodBase.GetCurrentMethod().Name, partitionName, partitionTypeNative))
                : partitionType;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "SetPartitionType", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int SetPartitionType_Native(nint serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, ImageStructures.PartitionType partitionType);

        public static void SetPartitionType(nint serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, ImageStructures.PartitionType partitionType)
        {
            int hr = SetPartitionType_Native(serviceHandle, storeId, partitionName, partitionType);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
                                MethodBase.GetCurrentMethod().Name, partitionName, hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "SetPartitionAttributes", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int SetPartitionAttributes_Native(nint serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, ImageStructures.PartitionAttributes attributes);

        public static void SetPartitionAttributes(nint serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, ImageStructures.PartitionAttributes attributes)
        {
            int hr = SetPartitionAttributes_Native(serviceHandle, storeId, partitionName, attributes);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
                                MethodBase.GetCurrentMethod().Name, partitionName, hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionAttributes", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionAttributes_Native(nint serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName, out ImageStructures.PartitionAttributes attributes);

        public static ImageStructures.PartitionAttributes GetPartitionAttributes(nint serviceHandle,
            ImageStructures.STORE_ID storeId, string partitionName)
        {
            _ = new ImageStructures.PartitionAttributes();

            int attributesNative = GetPartitionAttributes_Native(serviceHandle, storeId, partitionName, out ImageStructures.PartitionAttributes attributes);
            return Win32Exports.FAILED(attributesNative)
                ? throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}",
                    MethodBase.GetCurrentMethod().Name, partitionName, attributesNative))
                : attributes;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "SetDiskAttributes", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int SetDiskAttributes_Native(nint serviceHandle, nint diskHandle,
            out ImageStructures.SetDiskAttributes attributes);

        public static void SetDiskAttributes(nint serviceHandle, nint diskHandle,
            ImageStructures.SetDiskAttributes attributes)
        {
            int hr = SetDiskAttributes_Native(serviceHandle, diskHandle, out _);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0} failed: {1:x}", MethodBase.GetCurrentMethod().Name,
                                hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "DismountVirtualHardDisk",
            CallingConvention = CallingConvention.StdCall)]
        private static extern int DismountVirtualHardDisk_Native(nint service, ImageStructures.STORE_ID storeId,
            [MarshalAs(UnmanagedType.Bool)] bool removeAccessPaths, [MarshalAs(UnmanagedType.Bool)] bool deleteFile,
            [MarshalAs(UnmanagedType.Bool)] bool fFailIfDiskMissing);

        public static void DismountVirtualHardDisk(nint service, ImageStructures.STORE_ID storeId,
            bool removeAccessPaths, bool deleteFile, bool failIfDiskMissing = false)
        {
            int hr = DismountVirtualHardDisk_Native(service, storeId, removeAccessPaths, deleteFile, failIfDiskMissing);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                                hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "DismountVirtualHardDiskByFileName",
            CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern int DismountVirtualHardDiskByFileName_Native(nint service, string fileName,
            [MarshalAs(UnmanagedType.Bool)] bool deleteFile);

        public static void DismountVirtualHardDiskByName(nint service, string fileName, bool deleteFile)
        {
            int hr = DismountVirtualHardDiskByFileName_Native(service, fileName, deleteFile);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                                hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetSectorSize",
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetSectorSize_Native(nint service, ImageStructures.STORE_ID storeId,
            out uint bytesPerSector);

        public static uint GetSectorSize(nint service, ImageStructures.STORE_ID storeId)
        {
            int sectorSizeNative = GetSectorSize_Native(service, storeId, out uint bytesPerSector);
            return Win32Exports.FAILED(sectorSizeNative)
                ? throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    sectorSizeNative))
                : bytesPerSector;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionOffset", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionOffset_Native(nint service, ImageStructures.STORE_ID storeId,
            string partitionName, out ulong startingSector);

        public static ulong GetPartitionOffset(nint service, ImageStructures.STORE_ID storeId, string partitionName)
        {
            int partitionOffsetNative = GetPartitionOffset_Native(service, storeId, partitionName, out ulong startingSector);
            return Win32Exports.FAILED(partitionOffsetNative)
                ? throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    partitionOffsetNative))
                : startingSector;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionSize", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPartitionSize_Native(nint service, ImageStructures.STORE_ID storeId,
            string partitionName, out ulong sectorCount);

        public static ulong GetPartitionSize(nint service, ImageStructures.STORE_ID storeId, string partitionName)
        {
            int partitionSizeNative = GetPartitionSize_Native(service, storeId, partitionName, out ulong sectorCount);
            return Win32Exports.FAILED(partitionSizeNative)
                ? throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    partitionSizeNative))
                : sectorCount;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetFreeBytesOnVolume", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int GetFreeBytesOnVolume_Native(nint service, ImageStructures.STORE_ID storeId,
            string partitionName, out ulong freeBytes);

        public static ulong GetFreeBytesOnVolume(nint service, ImageStructures.STORE_ID storeId, string partitionName)
        {
            int bytesOnVolumeNative = GetFreeBytesOnVolume_Native(service, storeId, partitionName, out ulong freeBytes);
            return Win32Exports.FAILED(bytesOnVolumeNative)
                ? throw new ImageStorageException(string.Format("{0} failed for partition {1}: {2:x}.",
                    MethodBase.GetCurrentMethod().Name, partitionName, bytesOnVolumeNative))
                : freeBytes;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "OpenVolume", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int OpenVolumeHandle_Native(nint service, ImageStructures.STORE_ID storeId,
            string partitionName, uint requestedAccess, uint shareMode, out nint volumeHandle);

        public static SafeFileHandle OpenVolumeHandle(nint service, ImageStructures.STORE_ID storeId,
            string partitionName, FileAccess access, FileShare share)
        {
            uint shareMode = 0;
            uint requestedAccess = 0;
            if ((access & FileAccess.Read) != 0)
            {
                requestedAccess |= 2147483648U;
            }

            if ((access & FileAccess.Write) != 0)
            {
                requestedAccess |= 1073741824U;
            }

            if ((access & FileAccess.ReadWrite) != 0)
            {
                requestedAccess = requestedAccess | 2147483648U | 1073741824U;
            }

            if ((share & FileShare.Read) != FileShare.None)
            {
                shareMode |= 1U;
            }

            if ((share & FileShare.Write) != FileShare.None)
            {
                shareMode |= 2U;
            }

            if ((share & FileShare.ReadWrite) != FileShare.None)
            {
                shareMode = shareMode | 1U | 2U;
            }

            if ((share & FileShare.Delete) != FileShare.None)
            {
                shareMode |= 4U;
            }

            int hr = OpenVolumeHandle_Native(service, storeId, partitionName, requestedAccess, shareMode,
                            out nint volumeHandle);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                                hr));
            }

            SafeFileHandle safeFileHandle = new(volumeHandle, true);
            return !safeFileHandle.IsInvalid
                ? safeFileHandle
                : throw new ImageStorageException(string.Format("{0} returned an invalid handle.",
                MethodBase.GetCurrentMethod().Name));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "CloseVolumeHandle",
            CallingConvention = CallingConvention.StdCall)]
        private static extern int CloseVolumeHandle_Native(nint service, nint volumeHandle);

        public static void CloseVolumeHandle(nint service, nint volumeHandle)
        {
            int hr = CloseVolumeHandle_Native(service, volumeHandle);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                                hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "LockAndDismountVolumeByHandle",
            CallingConvention = CallingConvention.StdCall)]
        private static extern int LockAndDismountVolumeByHandle_Native(nint service, SafeFileHandle volumeHandle,
            [MarshalAs(UnmanagedType.Bool)] bool forceDismount);

        public static void LockAndDismountVolume(nint service, SafeFileHandle volumeHandle, bool forceDismount)
        {
            int hr = LockAndDismountVolumeByHandle_Native(service, volumeHandle, forceDismount);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                                hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "UnlockVolumeByHandle",
            CallingConvention = CallingConvention.StdCall)]
        private static extern int UnlockVolumeByHandle_Native(nint service, nint volumeHandle);

        public static void UnlockVolume(nint service, SafeHandle volumeHandle)
        {
            int hr = UnlockVolumeByHandle_Native(service, volumeHandle.DangerousGetHandle());
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                                hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "FormatPartition", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int FormatPartition_Native(nint service, ImageStructures.STORE_ID storeId,
            string partitionName, string fileSystem, uint cbClusterSize);

        public static void FormatPartition(nint service, ImageStructures.STORE_ID storeId, string partitionName,
            string fileSystem, uint cbClusterSize)
        {
            int hr = FormatPartition_Native(service, storeId, partitionName, fileSystem, cbClusterSize);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}.",
                                MethodBase.GetCurrentMethod().Name, partitionName, hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "AttachWOFToVolume", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AttachWOFToVolume_Native(nint service, string volumePath);

        public static void AttachWOFToVolume(nint service, string volumePath)
        {
            int volumeNative = AttachWOFToVolume_Native(service, volumePath);
            if (Win32Exports.FAILED(volumeNative))
            {
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}.",
                                MethodBase.GetCurrentMethod().Name, volumePath, volumeNative));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "AddAccessPath", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AddAccessPath_Native(nint service, ImageStructures.STORE_ID storeId,
            string partitionName, string accessPath);

        public static void AddAccessPath(nint service, ImageStructures.STORE_ID storeId, string partitionName,
            string accessPath)
        {
            int hr = AddAccessPath_Native(service, storeId, partitionName, accessPath);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}.",
                                MethodBase.GetCurrentMethod().Name, partitionName, hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "WaitForVolumeArrival", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int WaitForVolumeArrival_Native(nint service, ImageStructures.STORE_ID storeId,
            string partitionName, int timeout);

        public static void WaitForVolumeArrival(nint service, ImageStructures.STORE_ID storeId, string partitionName,
            int timeout)
        {
            int hr = WaitForVolumeArrival_Native(service, storeId, partitionName, timeout);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0}({1}) failed: {2:x}.",
                                MethodBase.GetCurrentMethod().Name, partitionName, hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "ReadFromDisk", CharSet = CharSet.Unicode)]
        private static extern int ReadFromDisk_Native(nint service, ImageStructures.STORE_ID storeId,
            ulong diskOffset, uint byteCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            byte[] buffer);

        public static void ReadFromDisk(nint service, ImageStructures.STORE_ID storeId, ulong diskOffset,
            byte[] buffer)
        {
            int hr = ReadFromDisk_Native(service, storeId, diskOffset, (uint)buffer.Length, buffer);
            if (Win32Exports.FAILED(hr))
            {
                throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                                MethodBase.GetCurrentMethod().Name, hr));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetPartitionStyle", CharSet = CharSet.Unicode)]
        private static extern int GetPartitionStyle_Native(nint service, SafeFileHandle hStore,
            out uint partitionStyle);

        public static uint GetPartitionStyle(nint service, SafeFileHandle storeHandle)
        {
            int partitionStyleNative = GetPartitionStyle_Native(service, storeHandle, out uint partitionStyle);
            return Win32Exports.FAILED(partitionStyleNative)
                ? throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    partitionStyleNative))
                : partitionStyle;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetSectorCount", CharSet = CharSet.Unicode)]
        private static extern int GetSectorCount_Native(nint service, SafeFileHandle hStore, out ulong sectorCount);

        public static ulong GetSectorCount(nint service, SafeFileHandle storeHandle)
        {
            int sectorCountNative = GetSectorCount_Native(service, storeHandle, out ulong sectorCount);
            return Win32Exports.FAILED(sectorCountNative)
                ? throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    sectorCountNative))
                : sectorCount;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetSectorSizeFromHandle", CharSet = CharSet.Unicode)]
        private static extern int GetSectorSizeFromHandle_Native(nint service, SafeFileHandle hStore,
            out uint sectorCount);

        public static uint GetSectorSize(nint service, SafeFileHandle storeHandle)
        {
            int fromHandleNative = GetSectorSizeFromHandle_Native(service, storeHandle, out uint sectorCount);
            return Win32Exports.FAILED(fromHandleNative)
                ? throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                    fromHandleNative))
                : sectorCount;
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "GetBlockAllocationBitmap", CharSet = CharSet.Unicode)]
        private static extern int GetBlockAllocationBitmap_Native(nint service, ImageStructures.STORE_ID storeId,
            string partitionName, uint blockSize, byte[] blockBitmapBuffer, uint bitmapBufferSize);

        public static void GetBlockAllocationBitmap(nint service, ImageStructures.STORE_ID storeId,
            string partitionName, uint blockSize, byte[] blockBitmapBuffer)
        {
            nint service1 = service;
            ImageStructures.STORE_ID storeId1 = storeId;
            string partitionName1 = partitionName;
            int num = (int)blockSize;
            byte[] blockBitmapBuffer1 = blockBitmapBuffer;
            int length = blockBitmapBuffer1.Length;
            int allocationBitmapNative = GetBlockAllocationBitmap_Native(service1, storeId1, partitionName1, (uint)num,
                blockBitmapBuffer1, (uint)length);
            if (Win32Exports.FAILED(allocationBitmapNative))
            {
                throw new ImageStorageException(string.Format("{0} failed: {1:x}.", MethodBase.GetCurrentMethod().Name,
                                allocationBitmapNative));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "WaitForPartitions", CharSet = CharSet.Unicode)]
        private static extern int WaitForPartitions_Native(nint service, ImageStructures.STORE_ID storeId,
            uint partitionCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
            ImageStructures.PARTITION_ENTRY[] partitions);

        public static nint WaitForPartitions(nint service, ImageStructures.STORE_ID storeId,
            ImageStructures.PARTITION_ENTRY[] partitions)
        {
            nint zero = nint.Zero;
            int hr = WaitForPartitions_Native(service, storeId, (uint)partitions.Length, partitions);
            return !Win32Exports.FAILED(hr)
                ? zero
                : throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                MethodBase.GetCurrentMethod().Name, hr));
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "CreateUsnJournal", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int CreateUsnJournal_Native(nint serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName);

        public static void CreateUsnJournal(nint serviceHandle, ImageStructures.STORE_ID storeId,
            string partitionName)
        {
            int usnJournalNative = CreateUsnJournal_Native(serviceHandle, storeId, partitionName);
            if (Win32Exports.FAILED(usnJournalNative))
            {
                throw new ImageStorageException(string.Format("{0}({1} failed with error code: {2:x}",
                                MethodBase.GetCurrentMethod().Name, partitionName, usnJournalNative));
            }
        }

        [DllImport("ImageStorageService.dll", EntryPoint = "CreateJunction", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int CreateJunction_Native(nint serviceHandle, ImageStructures.STORE_ID storeId,
            string sourceName, string targetPartition, string targetPath,
            [MarshalAs(UnmanagedType.U1)] bool useWellKnownGuids);

        public static void CreateJunction(nint serviceHandle, ImageStructures.STORE_ID storeId, string sourceName,
            string targetPartition, string targetName, bool useWellKnownGuids = false)
        {
            int junctionNative = CreateJunction_Native(serviceHandle, storeId, sourceName, targetPartition, targetName,
                useWellKnownGuids);
            if (Win32Exports.FAILED(junctionNative))
            {
                throw new ImageStorageException(string.Format("{0} failed with error code: {1:x}",
                                MethodBase.GetCurrentMethod().Name, junctionNative));
            }
        }
    }
}