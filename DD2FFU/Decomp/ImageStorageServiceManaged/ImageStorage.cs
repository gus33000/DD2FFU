// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.ImageStorage
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using DD2FFU;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.Win32.SafeHandles;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    
    public class ImageStorage
    {
        private LogFunction _logDebug;
        private readonly LogFunction _logError;
        private LogFunction _logInfo;
        private LogFunction _logWarning;
        private readonly ImageStorageManager _manager;
        private readonly List<string> _pathsToRemove;
        private ImageStructures.STORE_ID _storeId;

        public ImageStorage(IULogger logger, ImageStorageManager manager)
        {
            Logger = logger;
            _manager = manager;
            _logError = LogError;
            ServiceHandle = new NativeServiceHandle(_logError);
            _storeId = new ImageStructures.STORE_ID();
            _pathsToRemove = new List<string>();
            IsMainOSStorage = true;
            PrepareLogging();
        }

        public ImageStorage(IULogger logger, ImageStorageManager manager, ImageStructures.STORE_ID storeId)
        {
            Logger = logger;
            _manager = manager;
            _logError = LogError;
            ServiceHandle = new NativeServiceHandle(_logError);
            _storeId = storeId;
            _pathsToRemove = new List<string>();
            IsMainOSStorage = true;
            PrepareLogging();
        }

        public IntPtr StoreHandle => SafeStoreHandle.DangerousGetHandle();

        public SafeFileHandle SafeStoreHandle { get; private set; }

        public bool IsMainOSStorage { get; private set; }

        public ImageStructures.STORE_ID StoreId
        {
            get => _storeId;
            set => _storeId = value;
        }

        public uint VirtualHardDiskSectorSize { get; set; }

        public string VirtualDiskFilePath { get; private set; }

        public IULogger Logger { get; }

        internal FullFlashUpdateImage Image { get; private set; }

        internal FullFlashUpdateImage.FullFlashUpdateStore Store { get; private set; }

        internal NativeServiceHandle ServiceHandle { get; }

        internal uint BytesPerBlock => ImageConstants.PAYLOAD_BLOCK_SIZE;

        internal bool ReadOnlyVirtualDisk { get; private set; }

        private uint ImageSectorCount { get; set; }

        private bool PostProcessVHD { get; set; }

        public void Cleanup()
        {
            CleanupTemporaryPaths();
            Image = null;
            Store = null;
            SafeStoreHandle = null;
        }

        public void LogError(string message)
        {
            Logger.LogError("{0}", (object) message);
        }

        public void LogWarning(string message)
        {
            Logger.LogWarning("{0}", (object) message);
        }

        public void LogInfo(string message)
        {
            Logger.LogInfo("{0}", (object) message);
        }

        public void LogDebug(string message)
        {
            Logger.LogDebug("{0}", (object) message);
        }

        public void SetFullFlashImage(FullFlashUpdateImage image)
        {
            Image = image;
        }

        public void CreateJunction(string sourceName, string targetPartition, string targetPath)
        {
            CreateJunction(sourceName, targetPartition, targetPath, false);
        }

        public void CreateJunction(string sourceName, string targetPartition, string targetPath, bool useWellKnownGuids)
        {
            NativeImaging.CreateJunction((IntPtr) ServiceHandle, StoreId, sourceName, targetPartition, targetPath,
                useWellKnownGuids);
        }

        public void SetFullFlashUpdateStore(FullFlashUpdateImage.FullFlashUpdateStore store)
        {
            if (Image != null)
                throw new ImageStorageException("ImageStorage already has a FullFlashUpdateImage.");
            if (Store != null)
                throw new ImageStorageException("ImageStorage already has a FullFlashUpdateStore.");
            Image = store.Image;
            Store = store;
        }

        public void DetachVirtualHardDisk(bool deleteFile)
        {
            NativeImaging.DismountVirtualHardDisk((IntPtr) ServiceHandle, _storeId, true, deleteFile, true);
        }

        public void CreateVirtualHardDiskFromStore(FullFlashUpdateImage.FullFlashUpdateStore store, string imagePath,
            uint partitionStyle, bool preparePartitions, ImageStructures.STORE_ID[] storeIds)
        {
            Image = store.Image;
            Store = store;
            IsMainOSStorage = store.IsMainOSStore;
            if (string.IsNullOrEmpty(imagePath))
                imagePath = CreateBackingVhdFileName(store.SectorSize);
            VirtualDiskFilePath = imagePath;
            var partitions1 = store.Partitions;
            var stringList = new List<string>();
            ImageSectorCount = store.MinSectorCount <= 0U
                ? (uint) (10737418240UL / store.SectorCount)
                : store.MinSectorCount;
            var count = partitions1.Count;
            if ((int) partitionStyle == (int) ImageConstants.PartitionTypeMbr)
                ++count;
            uint num1 = 1;
            if (ImageConstants.MINIMUM_PARTITION_SIZE > store.SectorSize)
                num1 = ImageConstants.MINIMUM_PARTITION_SIZE / store.SectorSize;
            foreach (var partition in store.Partitions)
            {
                if (partition.TotalSectors < num1 && !partition.UseAllSpace)
                    partition.TotalSectors = num1;
                if (string.Compare(partition.PrimaryPartition, partition.Name, true, CultureInfo.InvariantCulture) != 0)
                {
                    if (stringList.Contains(partition.Name))
                        throw new ImageStorageException(string.Format(
                            "{0}: A duplicate partition cannot be used as a primary partition for another duplicate partition.",
                            MethodBase.GetCurrentMethod().Name));
                    if (!stringList.Contains(partition.PrimaryPartition))
                        stringList.Add(partition.PrimaryPartition);
                    stringList.Add(partition.Name);
                }
            }

            var index1 = -1;
            var partitions2 = new ImageStructures.PARTITION_ENTRY[count];
            for (var index2 = 0; index2 < partitions2.Length; ++index2)
            {
                var index3 = index2;
                if (index2 == partitions2.Length - 1 && (int) partitionStyle == (int) ImageConstants.PartitionTypeMbr)
                {
                    if (index1 == -1)
                    {
                        partitions2[index2].PartitionName = ImageConstants.MBR_METADATA_PARTITION_NAME;
                        partitions2[index2].FileSystem = "";
                        partitions2[index2].SectorCount = ImageConstants.MBR_METADATA_PARTITION_SIZE / store.SectorSize;
                        partitions2[index2].MBRFlags = 0;
                        partitions2[index2].MBRType = ImageConstants.MBR_METADATA_PARTITION_TYPE;
                        partitions2[index2].AlignmentSizeInBytes = store.SectorSize;
                        continue;
                    }

                    index3 = index1;
                }

                var partition = partitions1[index3];
                if (partition.UseAllSpace && (int) partitionStyle == (int) ImageConstants.PartitionTypeMbr &&
                    index3 != index1)
                {
                    if (index1 != -1)
                        throw new ImageStorageException(string.Format(
                            "There are two partition set to use all remaining space on disk: {0} and {1}",
                            store.Partitions[index3].Name, store.Partitions[index1].Name));
                    index1 = index3;
                    partitions2[index2].PartitionName = ImageConstants.MBR_METADATA_PARTITION_NAME;
                    partitions2[index2].FileSystem = "";
                    partitions2[index2].SectorCount = ImageConstants.MBR_METADATA_PARTITION_SIZE / store.SectorSize;
                    partitions2[index2].MBRFlags = 0;
                    partitions2[index2].MBRType = ImageConstants.MBR_METADATA_PARTITION_TYPE;
                    partitions2[index2].AlignmentSizeInBytes = store.SectorSize;
                }
                else
                {
                    ValidatePartitionStrings(partition);
                    var alignmentInBytes = store.SectorSize;
                    if (partition.ByteAlignment != 0U)
                    {
                        if (partition.ByteAlignment < store.SectorSize)
                            throw new ImageStorageException(string.Format(
                                "{0}: The alignment for partition '{1}' is smaller than the sector size: 0x{2:x}/0x{3:x}.",
                                (object) MethodBase.GetCurrentMethod().Name, (object) partition.Name,
                                (object) partition.ByteAlignment, (object) store.SectorSize));
                        alignmentInBytes = partition.ByteAlignment;
                    }
                    else if (Image.DefaultPartitionAlignmentInBytes > store.SectorSize)
                    {
                        alignmentInBytes = Image.DefaultPartitionAlignmentInBytes;
                    }

                    PreparePartitionEntry(ref partitions2[index2], store, partition, partitionStyle, alignmentInBytes);
                }
            }

            ulong num2 = 0;
            var partitionEntryArray = partitions2;
            if (partitionEntryArray[partitionEntryArray.Length - 1].SectorCount == uint.MaxValue)
            {
                for (var index2 = 0; index2 < partitions2.Length; ++index2)
                {
                    if ((int) partitionStyle == (int) ImageConstants.PartitionTypeMbr && index1 != -1 && index2 >= 3)
                        num2 += 65536U / store.SectorSize;
                    var num3 = partitions2[index2].AlignmentSizeInBytes / store.SectorSize;
                    if (num2 == 0UL || num2 % num3 != 0UL)
                        num2 += num3 - num2 % num3;
                    if (index2 != partitions2.Length - 1)
                        num2 += partitions2[index2].SectorCount;
                    else
                        break;
                }

                uint num4 = 0;
                uint num5;
                if ((int) partitionStyle == (int) ImageConstants.PartitionTypeGpt)
                {
                    num5 = num4 + 2U * ImageConstants.PARTITION_TABLE_METADATA_SIZE;
                }
                else
                {
                    num5 = ImageConstants.PAYLOAD_BLOCK_SIZE;
                    if (partitions2.Length > 3)
                        num5 += (uint) (partitions2.Length - 3) * ImageConstants.PAYLOAD_BLOCK_SIZE;
                }

                num2 += num5 / store.SectorSize;
            }

            if (num2 > ImageSectorCount)
                throw new ImageStorageException(
                    "The store's minSectorCount is less than the count of sectors in its partitions.");
            for (var index2 = 0; index2 < partitions2.Length; ++index2)
                if (partitions2[index2].SectorCount == uint.MaxValue)
                {
                    partitions2[index2].SectorCount = ImageSectorCount - num2;
                    var num3 = partitions2[index2].SectorCount * store.SectorSize;
                    if (num3 % ImageConstants.PAYLOAD_BLOCK_SIZE != 0UL)
                    {
                        var num4 = (long) num3;
                        var num5 = (ulong) num4 - (ulong) num4 % ImageConstants.PAYLOAD_BLOCK_SIZE;
                        partitions2[index2].SectorCount = num5 / store.SectorSize;
                        if (partitions2[index2].SectorCount == 0UL)
                            throw new ImageStorageException(
                                "The store's minSectorCount is less than the count of sectors in its partitions.");
                    }

                    break;
                }

            try
            {
                var maxSizeInBytes = ImageSectorCount * (ulong) store.SectorSize;
                CleanupAllMountedDisks();
                SafeStoreHandle = new SafeFileHandle(
                    NativeImaging.InitializeVirtualHardDisk((IntPtr) ServiceHandle, VirtualDiskFilePath, maxSizeInBytes,
                        ref _storeId, partitions2, preparePartitions, store.IsMainOSStore, VirtualHardDiskSectorSize,
                        storeIds), true);
            }
            catch (ImageStorageException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ImageStorageException(
                    string.Format("{0}: Unable to create the VHD.", MethodBase.GetCurrentMethod().Name), ex);
            }

            if (store.IsMainOSStore)
                _pathsToRemove.Add(GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME));
            PostProcessVHD = true;
        }

        public void MountExistingVirtualHardDisk(string imagePath, bool readOnly)
        {
            _storeId = new ImageStructures.STORE_ID();
            ReadOnlyVirtualDisk = readOnly;
            VirtualDiskFilePath = imagePath;
            try
            {
                using (var virtualDisk = new DynamicHardDisk(imagePath, false))
                {
                    using (var virtualDiskStream = new VirtualDiskStream(virtualDisk))
                    {
                        var masterBootRecord = new MasterBootRecord(Logger, (int) virtualDisk.SectorSize);
                        masterBootRecord.ReadFromStream(virtualDiskStream, MasterBootRecord.MbrParseType.Normal);
                        if (masterBootRecord.IsValidProtectiveMbr())
                        {
                            var guidPartitionTable = new GuidPartitionTable((int) virtualDisk.SectorSize, Logger);
                            guidPartitionTable.ReadFromStream(virtualDiskStream, true);
                            var flag = false;
                            foreach (var entry in guidPartitionTable.Entries)
                                if (string.Compare(entry.PartitionName, ImageConstants.MAINOS_PARTITION_NAME, true,
                                        CultureInfo.InvariantCulture) == 0)
                                    flag = true;
                            if (!flag)
                                throw new ImageStorageException(string.Format(
                                    "{0}: The given VHD does not contain the partition '{1}'.",
                                    MethodBase.GetCurrentMethod().Name, ImageConstants.MAINOS_PARTITION_NAME));
                            _storeId.StoreType = ImageConstants.PartitionTypeGpt;
                            _storeId.StoreId_GPT = guidPartitionTable.Header.DiskId;
                        }
                        else
                        {
                            if (masterBootRecord.FindPartitionByName(ImageConstants.MAINOS_PARTITION_NAME) == null)
                                throw new ImageStorageException(string.Format(
                                    "{0}: The given VHD does not contain the partition '{1}'.",
                                    MethodBase.GetCurrentMethod().Name, ImageConstants.MAINOS_PARTITION_NAME));
                            _storeId.StoreType = ImageConstants.PartitionTypeMbr;
                            _storeId.StoreId_MBR = masterBootRecord.DiskSignature;
                        }
                    }
                }
            }
            catch (ImageStorageException ex)
            {
            }

            NativeImaging.DismountVirtualHardDisk((IntPtr) ServiceHandle, StoreId, false, false, false);
            SafeStoreHandle =
                new SafeFileHandle(
                    NativeImaging.OpenVirtualHardDisk((IntPtr) ServiceHandle, imagePath, out _storeId, readOnly), true);
            var empty = string.Empty;
            string str;
            try
            {
                str = BuildPaths.GetImagingTempPath(Path.GetTempPath()) + ".mnt\\";
                Directory.CreateDirectory(str);
                _pathsToRemove.Add(str);
            }
            catch (SecurityException ex)
            {
                throw new ImageStorageException("Unable to retrieve a temporary path.", ex);
            }

            NativeImaging.AddAccessPath((IntPtr) ServiceHandle, _storeId, ImageConstants.MAINOS_PARTITION_NAME, str);
        }

        public void DismountVirtualHardDisk(bool skipPostProcessing)
        {
            DismountVirtualHardDisk(skipPostProcessing, false);
        }

        public void DismountVirtualHardDisk(bool skipPostProcessing, bool normalizeDiskSignature)
        {
            var tickCount = Environment.TickCount;
            if (SafeStoreHandle == null)
            {
                Logger.DebugLogger("{0}: This function was called when no image is mounted.",
                    (object) MethodBase.GetCurrentMethod().Name);
            }
            else
            {
                if (!ReadOnlyVirtualDisk && !skipPostProcessing)
                {
                    Logger.LogInfo("{0}:[{1}] Enabling USN journal on partition {2}.",
                        (object) MethodBase.GetCurrentMethod().Name,
                        (object) ((double) (Environment.TickCount - tickCount) / 1000.0),
                        (object) ImageConstants.MAINOS_PARTITION_NAME);
                    NativeImaging.CreateUsnJournal((IntPtr) ServiceHandle, StoreId,
                        ImageConstants.MAINOS_PARTITION_NAME);
                    Logger.LogInfo("{0}:[{1}] Enabling USN journal on partition {2}.",
                        (object) MethodBase.GetCurrentMethod().Name,
                        (object) ((double) (Environment.TickCount - tickCount) / 1000.0),
                        (object) ImageConstants.DATA_PARTITION_NAME);
                    NativeImaging.CreateUsnJournal((IntPtr) ServiceHandle, StoreId, ImageConstants.DATA_PARTITION_NAME);
                    if ((int) StoreId.StoreType == (int) ImageConstants.PartitionTypeMbr)
                    {
                        Logger.LogInfo("{0}:[{1}] Updating the BCD.", (object) MethodBase.GetCurrentMethod().Name,
                            (object) ((double) (Environment.TickCount - tickCount) / 1000.0));
                        UpdateBootConfigurationDatabase(ImageConstants.BCD_FILE_PATH,
                            ImageConstants.SYSTEM_STORE_SIGNATURE);
                    }

                    if (normalizeDiskSignature)
                    {
                        NativeImaging.WriteMountManagerRegistry2((IntPtr) ServiceHandle, _storeId, true);
                        NativeImaging.NormalizeVolumeMountPoints((IntPtr) ServiceHandle, _storeId,
                            GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME));
                    }
                }

                SafeStoreHandle.Close();
                SafeStoreHandle = null;
                NativeImaging.DismountVirtualHardDisk((IntPtr) ServiceHandle, _storeId, false, false, true);
                if (!ReadOnlyVirtualDisk && !skipPostProcessing && PostProcessVHD && Image != null)
                {
                    var fileSystem = (string) null;
                    var bootPartitionName = (string) null;
                    foreach (var partition in Store.Partitions)
                        if (partition.Bootable)
                        {
                            fileSystem = partition.FileSystem;
                            bootPartitionName = partition.Name;
                            break;
                        }

                    Logger.LogInfo("{0}:[{1}] Making the virtual disk bootable.",
                        (object) MethodBase.GetCurrentMethod().Name,
                        (object) ((double) (Environment.TickCount - tickCount) / 1000.0));
                    PostProcessVirtualHardDisk(VirtualDiskFilePath, Logger, bootPartitionName, fileSystem,
                        normalizeDiskSignature);
                }

                Logger.LogInfo("{0}:[{1}] Cleaning up temporary paths.", (object) MethodBase.GetCurrentMethod().Name,
                    (object) ((double) (Environment.TickCount - tickCount) / 1000.0));
                CleanupTemporaryPaths();
                Image = null;
                Store = null;
                SafeStoreHandle = null;
                _storeId = new ImageStructures.STORE_ID();
                Logger.LogInfo("Storage Service: Dismounting the image in {0:F1} seconds.",
                    (object) ((double) (Environment.TickCount - tickCount) / 1000.0));
            }
        }

        public string GetPartitionPath(string partitionName)
        {
            var stringBuilder = new StringBuilder("path", 1024);
            if (_storeId.StoreId_GPT == Guid.Empty && _storeId.StoreId_MBR == 0U)
            {
                var partitionName1 = partitionName;
                var path = stringBuilder;
                var capacity = path.Capacity;
                NativeImaging.GetPartitionPathNoContext(partitionName1, path, (uint) capacity);
            }
            else
            {
                var service = (IntPtr) ServiceHandle;
                var storeId = _storeId;
                var partitionName1 = partitionName;
                var path = stringBuilder;
                var capacity = path.Capacity;
                NativeImaging.GetPartitionPath(service, storeId, partitionName1, path, (uint) capacity);
            }

            return stringBuilder.ToString();
        }

        public static string GetPartitionPathNoContext(string partitionName)
        {
            var stringBuilder = new StringBuilder("path", 1024);
            var partitionName1 = partitionName;
            var path = stringBuilder;
            var capacity = path.Capacity;
            NativeImaging.GetPartitionPathNoContext(partitionName1, path, (uint) capacity);
            return stringBuilder.ToString();
        }

        public string GetDiskName()
        {
            return NativeImaging.GetDiskName((IntPtr) ServiceHandle, StoreId);
        }

        public void SetDiskAttributes(ImageStructures.DiskAttributes attributes,
            ImageStructures.DiskAttributes attributesMask, bool persist)
        {
            var attributes1 = new ImageStructures.SetDiskAttributes();
            attributes1.Version = (uint) Marshal.SizeOf((object) attributes1);
            attributes1.Persist = persist ? (byte) 1 : (byte) 0;
            attributes1.AttributesMask = attributesMask;
            attributes1.Attributes = attributes;
            NativeImaging.SetDiskAttributes((IntPtr) ServiceHandle, StoreHandle, attributes1);
        }

        public void FormatPartition(string partitionName, string fileSystem, uint cbClusterSize)
        {
            NativeImaging.FormatPartition((IntPtr) ServiceHandle, _storeId, partitionName, fileSystem, cbClusterSize);
        }

        public void AttachWOFToVolume(string partitionName)
        {
            var stringBuilder = new StringBuilder(1024);
            var service = (IntPtr) ServiceHandle;
            var storeId = _storeId;
            var partitionName1 = partitionName;
            var path = stringBuilder;
            var capacity = path.Capacity;
            NativeImaging.GetPartitionPath(service, storeId, partitionName1, path, (uint) capacity);
            NativeImaging.AttachWOFToVolume((IntPtr) ServiceHandle, stringBuilder.ToString());
        }

        public Guid GetPartitionTypeGpt(string partitionName)
        {
            return NativeImaging.GetPartitionType((IntPtr) ServiceHandle, _storeId, partitionName).gptType;
        }

        public byte GetPartitionTypeMbr(string partitionName)
        {
            return NativeImaging.GetPartitionType((IntPtr) ServiceHandle, _storeId, partitionName).mbrType;
        }

        public void SetPartitionType(string partitionName, Guid partitionType)
        {
            NativeImaging.SetPartitionType((IntPtr) ServiceHandle, _storeId, partitionName,
                new ImageStructures.PartitionType
                {
                    gptType = partitionType
                });
        }

        public void SetPartitionType(string partitionName, byte partitionType)
        {
            NativeImaging.SetPartitionType((IntPtr) ServiceHandle, _storeId, partitionName,
                new ImageStructures.PartitionType
                {
                    mbrType = partitionType
                });
        }

        public ulong GetPartitionAttributesGpt(string partitionName)
        {
            if ((int) _storeId.StoreType != (int) ImageConstants.PartitionTypeGpt)
                throw new ImageStorageException(
                    "UInt64 GetPartitionAttributes(string) can only be called on an GPT style disk.");
            return NativeImaging.GetPartitionAttributes((IntPtr) ServiceHandle, _storeId, partitionName).gptAttributes;
        }

        public byte GetPartitionAttributesMbr(string partitionName)
        {
            if ((int) _storeId.StoreType != (int) ImageConstants.PartitionTypeMbr)
                throw new ImageStorageException(
                    "byte GetPartitionAttributes(string) can only be called on an MBR style disk.");
            return NativeImaging.GetPartitionAttributes((IntPtr) ServiceHandle, _storeId, partitionName).mbrAttributes;
        }

        public void SetPartitionAttributes(string partitionName, ulong attributes)
        {
            NativeImaging.SetPartitionAttributes((IntPtr) ServiceHandle, _storeId, partitionName,
                new ImageStructures.PartitionAttributes
                {
                    gptAttributes = attributes
                });
        }

        public ulong GetPartitionSize(string partitionName)
        {
            return NativeImaging.GetPartitionSize((IntPtr) ServiceHandle, _storeId, partitionName);
        }

        public string GetPartitionFileSystem(string partitionName)
        {
            return NativeImaging.GetPartitionFileSystem((IntPtr) ServiceHandle, StoreId, partitionName);
        }

        public bool IsPartitionTargeted(string partition)
        {
            return _manager.IsPartitionTargeted(partition);
        }

        public bool IsBackingFileVhdx()
        {
            return VirtualDiskFilePath.EndsWith(".vhdx", true, CultureInfo.InvariantCulture);
        }

        public ulong GetFreeBytesOnVolume(string partitionName)
        {
            return NativeImaging.GetFreeBytesOnVolume((IntPtr) ServiceHandle, _storeId, partitionName);
        }

        public SafeFileHandle OpenVolumeHandle(string partitionName)
        {
            return NativeImaging.OpenVolumeHandle((IntPtr) ServiceHandle, _storeId, partitionName, FileAccess.ReadWrite,
                FileShare.ReadWrite);
        }

        public SafeFileHandle OpenVolumeHandle(string partitionName, FileAccess access, FileShare share)
        {
            return NativeImaging.OpenVolumeHandle((IntPtr) ServiceHandle, _storeId, partitionName, access, share);
        }

        public void WaitForVolume(string strVolumeName)
        {
            WaitForVolume(strVolumeName, int.MaxValue);
        }

        public void WaitForVolume(string strVolumeName, int timeout)
        {
            NativeImaging.WaitForVolumeArrival((IntPtr) ServiceHandle, _storeId, strVolumeName, timeout);
        }

        public void LockAndDismountVolume(string partitionName)
        {
            LockAndDismountVolume(partitionName, false);
        }

        public void LockAndDismountVolume(string partitionName, bool forceDismount)
        {
            using (var safeVolumeHandle = new SafeVolumeHandle(this, partitionName))
            {
                NativeImaging.LockAndDismountVolume((IntPtr) ServiceHandle, safeVolumeHandle.VolumeHandle,
                    forceDismount);
            }
        }

        public void UnlockVolume(string partitionName)
        {
            using (var safeVolumeHandle = new SafeVolumeHandle(this, partitionName))
            {
                NativeImaging.UnlockVolume((IntPtr) ServiceHandle, safeVolumeHandle.VolumeHandle);
            }
        }

        public bool PartitionIsMountedRaw(string partitionName)
        {
            if (Constants.partitions.Any(x => x.ToLower() == partitionName.ToLower()))
                return true;
            return string.Compare("RAW", GetPartitionFileSystem(partitionName), true, CultureInfo.InvariantCulture) ==
                   0;
        }

        public void CreateUsnJournal(string partitionName)
        {
            NativeImaging.CreateUsnJournal((IntPtr) ServiceHandle, StoreId, partitionName);
        }

        internal void MountFullFlashImageStore(FullFlashUpdateImage.FullFlashUpdateStore store,
            PayloadReader payloadReader, StorePayload payload, bool randomizeGptIds)
        {
            VirtualDiskFilePath = CreateBackingVhdFileName(store.SectorSize);
            Image = store.Image;
            Store = store;
            var tickCount = Environment.TickCount;
            if (store.SectorSize > BytesPerBlock)
                throw new ImageStorageException(string.Format(
                    "The sector size (0x{0:x} bytes) is greater than the image block size (0x{1x} bytes)",
                    store.SectorSize, BytesPerBlock));
            if (BytesPerBlock % store.SectorSize != 0U)
                throw new ImageStorageException(string.Format(
                    "The block size (0x{0:x} bytes) is not a mulitple of the sector size (0x{1x} bytes)", BytesPerBlock,
                    store.SectorSize));
            var num = (ulong) store.SectorCount;
            if (Image == null)
                throw new ImageStorageException(string.Format("{0}: The full flash update image has not been set.",
                    MethodBase.GetCurrentMethod().Name));
            if (num == 0UL)
                num = 10737418240UL / store.SectorSize;
            _storeId.StoreType = store.Image.ImageStyle;
            if (store.Id != null)
            {
                if ((int) _storeId.StoreType == (int) ImageConstants.PartitionTypeGpt)
                    _storeId.StoreId_GPT = Guid.Parse(store.Id);
                else
                    _storeId.StoreId_MBR = Convert.ToUInt32(store.Id);
            }

            SafeStoreHandle =
                new SafeFileHandle(
                    NativeImaging.CreateEmptyVirtualDisk((IntPtr) ServiceHandle, VirtualDiskFilePath, ref _storeId,
                        num * store.SectorSize, VirtualHardDiskSectorSize), true);
            Logger.LogInfo("Storage Service: Created a new image in {0:F1} seconds.",
                (object) ((double) (Environment.TickCount - tickCount) / 1000.0));
            var empty = string.Empty;
            var str1 = string.Empty;
            var flag = false;
            foreach (var partition in store.Partitions)
                if (partition.Name.Equals(ImageConstants.SYSTEM_PARTITION_NAME, StringComparison.OrdinalIgnoreCase))
                {
                    if (!IsPartitionHidden(partition)) flag = true;
                    break;
                }

            string str2;
            try
            {
                var imagingTempPath = BuildPaths.GetImagingTempPath(Path.GetTempPath());
                str2 = imagingTempPath + ".mnt\\";
                Directory.CreateDirectory(str2);
                _pathsToRemove.Add(str2);
                if (flag)
                {
                    str1 = imagingTempPath + ".efiesp.mnt\\";
                    Directory.CreateDirectory(str1);
                    _pathsToRemove.Add(str1);
                }
            }
            catch (SecurityException ex)
            {
                throw new ImageStorageException("Unable to retrieve a temporary path.", ex);
            }

            try
            {
                MountFullFlashImageStoreInternal(store, payloadReader, payload, randomizeGptIds);
            }
            catch (Win32ExportException ex)
            {
                throw new ImageStorageException("Unable to mount the existing full flash update image.", ex);
            }

            if (!store.IsMainOSStore)
                return;
            NativeImaging.AddAccessPath((IntPtr) ServiceHandle, _storeId, ImageConstants.MAINOS_PARTITION_NAME, str2);
            if (flag)
                NativeImaging.AddAccessPath((IntPtr) ServiceHandle, _storeId, ImageConstants.SYSTEM_PARTITION_NAME,
                    str1);
            else
                Logger.LogDebug("{0}: Not mounting the system partition because it is absent or hidden",
                    (object) MethodBase.GetCurrentMethod().Name);
            if (!IsImageCompressed(str2))
                return;
            try
            {
                AttachWOFToVolume(ImageConstants.MAINOS_PARTITION_NAME);
                AttachWOFToVolume(ImageConstants.DATA_PARTITION_NAME);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(string.Format("{0}: Unable to attach WOF to a volume.",
                    MethodBase.GetCurrentMethod().Name));
            }
        }

        internal void AttachToMountedVirtualHardDisk(string physicalDiskPath, bool readOnly, bool isMainOSStore)
        {
            var imagePath = string.Empty;
            _storeId = new ImageStructures.STORE_ID();
            var storeHandle = IntPtr.Zero;
            NativeImaging.AttachToMountedImage((IntPtr) ServiceHandle, physicalDiskPath, readOnly, out imagePath,
                out _storeId, out storeHandle);
            SafeStoreHandle = new SafeFileHandle(storeHandle, true);
            if (isMainOSStore)
                WaitForVolume(ImageConstants.MAINOS_PARTITION_NAME);
            IsMainOSStorage = isMainOSStore;
            VirtualDiskFilePath = imagePath;
        }

        private string CreateBackingVhdFileName(uint sectorSize)
        {
            try
            {
                return Path.Combine(
                    Environment.GetEnvironmentVariable("VHDTMP") ??
                    Path.GetDirectoryName(BuildPaths.GetImagingTempPath(Path.GetTempPath())),
                    sectorSize != 512U
                        ? Guid.NewGuid().ToString("N") + ".vhdx"
                        : Guid.NewGuid().ToString("N") + ".vhd");
            }
            catch (SecurityException ex)
            {
                throw new ImageStorageException("Unable to retrieve a temporary path.", ex);
            }
        }

        private static void PostProcessVirtualHardDisk(string virtualImagePath, IULogger logger,
            string bootPartitionName, string fileSystem, bool normalizeDiskSignature)
        {
            var masterBootRecord1 = (MasterBootRecord) null;
            var flag = false;
            using (var virtualDisk = new DynamicHardDisk(virtualImagePath, true))
            {
                using (var virtualDiskStream = new VirtualDiskStream(virtualDisk))
                {
                    var masterBootRecord2 = new MasterBootRecord(logger, (int) virtualDisk.SectorSize);
                    masterBootRecord2.ReadFromStream(virtualDiskStream, MasterBootRecord.MbrParseType.Normal);
                    if (!masterBootRecord2.IsValidProtectiveMbr() && !string.IsNullOrEmpty(fileSystem) &&
                        !string.IsNullOrEmpty(bootPartitionName))
                    {
                        if (masterBootRecord2.FindPartitionByName(bootPartitionName) == null)
                            throw new ImageStorageException(string.Format(
                                "{0}: No bootable partition was found in the image.",
                                MethodBase.GetCurrentMethod().Name));
                        flag = true;
                    }

                    if (normalizeDiskSignature && (int) masterBootRecord2.DiskSignature !=
                        (int) ImageConstants.SYSTEM_STORE_SIGNATURE)
                    {
                        masterBootRecord2.DiskSignature = ImageConstants.SYSTEM_STORE_SIGNATURE;
                        flag = true;
                    }

                    if (flag)
                        masterBootRecord2.WriteToStream(virtualDiskStream, true);
                    masterBootRecord1 = null;
                }
            }
        }

        private void PrepareLogging()
        {
            _logWarning = LogWarning;
            _logInfo = LogInfo;
            _logDebug = LogDebug;
            NativeImaging.SetLoggingFunction((IntPtr) ServiceHandle, NativeImaging.LogLevel.levelWarning, _logWarning);
            NativeImaging.SetLoggingFunction((IntPtr) ServiceHandle, NativeImaging.LogLevel.levelInfo, _logInfo);
            NativeImaging.SetLoggingFunction((IntPtr) ServiceHandle, NativeImaging.LogLevel.levelDebug, _logDebug);
            LogInfo(string.Format("ETW Log Path: {0}", NativeImaging.GetETWLogPath((IntPtr) ServiceHandle)));
            LogInfo(string.Format("OS Version: {0}", Environment.OSVersion.VersionString));
        }

        private void CleanupTemporaryPaths()
        {
            foreach (var dirPath in _pathsToRemove)
            {
                Logger.LogInfo("{0}: Cleaning up temporary path {1}.", (object) MethodBase.GetCurrentMethod().Name,
                    (object) dirPath);
                FileUtils.DeleteTree(dirPath);
            }

            _pathsToRemove.Clear();
        }

        public void UpdateBootConfigurationDatabase(string bcdFile, uint diskSignature)
        {
            var save = false;
            var partitionOffset1 = NativeImaging.GetPartitionOffset((IntPtr) ServiceHandle, _storeId,
                ImageConstants.MAINOS_PARTITION_NAME);
            var partitionOffset2 = NativeImaging.GetPartitionOffset((IntPtr) ServiceHandle, _storeId,
                ImageConstants.SYSTEM_PARTITION_NAME);
            ulong partitionOffset3;
            ulong partitionOffset4;
            if (Image == null)
            {
                partitionOffset3 = partitionOffset1 * VirtualHardDiskSectorSize;
                partitionOffset4 = partitionOffset2 * VirtualHardDiskSectorSize;
            }
            else
            {
                partitionOffset3 = partitionOffset1 * Image.Stores[0].SectorSize;
                partitionOffset4 = partitionOffset2 * Image.Stores[0].SectorSize;
            }

            string str;
            try
            {
                str = GetPartitionPath(ImageConstants.SYSTEM_PARTITION_NAME) + bcdFile;
            }
            catch (ImageStorageException ex)
            {
                Logger.LogInfo("{0}: Not updating the BCD - unable to find the '{1}' partition.",
                    (object) MethodBase.GetCurrentMethod().Name, (object) ImageConstants.SYSTEM_PARTITION_NAME);
                return;
            }

            if (!File.Exists(str))
            {
                Logger.LogInfo("{0}: Not updating the BCD - unable to find the path: {1}",
                    (object) MethodBase.GetCurrentMethod().Name, (object) str);
            }
            else
            {
                var simpleMbr1 = PartitionIdentifierEx.CreateSimpleMbr(partitionOffset3, diskSignature);
                var simpleMbr2 = PartitionIdentifierEx.CreateSimpleMbr(partitionOffset4, diskSignature);
                using (var configurationDatabase = new BootConfigurationDatabase(str))
                {
                    configurationDatabase.Mount();
                    var bcdObject1 = configurationDatabase.GetObject(BcdObjects.WindowsLoader);
                    var bcdObject2 = configurationDatabase.GetObject(BcdObjects.BootManager);
                    var bcdObject3 = configurationDatabase.GetObject(BcdObjects.UpdateOSWim);
                    var bcdObject4 = configurationDatabase.GetObject(BcdObjects.WindowsSetupRamdiskOptions);
                    if (bcdObject2 == null)
                        throw new ImageStorageException(string.Format("{0}: The Boot Manager Object was not found.",
                            MethodBase.GetCurrentMethod().Name));
                    for (var index1 = 0; index1 < configurationDatabase.Objects.Count; ++index1)
                    {
                        var bcdObject5 = configurationDatabase.Objects[index1];
                        for (var index2 = 0; index2 < bcdObject5.Elements.Count; ++index2)
                        {
                            var element = bcdObject5.Elements[index2];
                            if (element.DataType.Format == ElementFormat.Device)
                            {
                                var bcdElementDevice = element as BcdElementDevice;
                                if (bcdElementDevice == null)
                                    throw new ImageStorageException(string.Format(
                                        "{0}: The default application's device element is invalid.",
                                        MethodBase.GetCurrentMethod().Name));
                                if (bcdElementDevice.BootDevice.Type == BcdElementBootDevice.DeviceType.BlockIo)
                                {
                                    if (bcdObject3 != null && bcdObject5.Id == bcdObject3.Id)
                                    {
                                        bcdElementDevice.ReplaceRamDiskDeviceIdentifier(simpleMbr1);
                                        configurationDatabase.SaveElementValue(bcdObject5, element);
                                        save = true;
                                    }
                                }
                                else if (bcdElementDevice.BootDevice.Type == BcdElementBootDevice.DeviceType.Boot &&
                                         (bcdElementDevice.DataType.Equals(BcdElementDataTypes.OsLoaderDevice) ||
                                          bcdElementDevice.DataType.Equals(BcdElementDataTypes.OsLoaderType) ||
                                          bcdElementDevice.DataType.Equals(BcdElementDataTypes.RamDiskSdiDevice)))
                                {
                                    if (bcdObject5.Id == bcdObject2.Id)
                                    {
                                        bcdElementDevice.ReplaceBootDeviceIdentifier(simpleMbr2);
                                        configurationDatabase.SaveElementValue(bcdObject5, element);
                                        save = true;
                                    }
                                    else if (bcdObject1 != null && bcdObject5.Id == bcdObject1.Id)
                                    {
                                        bcdElementDevice.ReplaceBootDeviceIdentifier(simpleMbr1);
                                        configurationDatabase.SaveElementValue(bcdObject5, element);
                                        save = true;
                                    }
                                    else if (bcdObject4 != null && bcdObject5.Id == bcdObject4.Id)
                                    {
                                        bcdElementDevice.ReplaceBootDeviceIdentifier(simpleMbr2);
                                        configurationDatabase.SaveElementValue(bcdObject5, element);
                                        save = true;
                                    }
                                    else
                                    {
                                        Logger.LogInfo(
                                            "{0}: Modifying unknown object device elements to point to the system partition. ID is {1}",
                                            (object) MethodBase.GetCurrentMethod().Name, (object) bcdObject5.Id);
                                        bcdElementDevice.ReplaceBootDeviceIdentifier(simpleMbr2);
                                        configurationDatabase.SaveElementValue(bcdObject5, element);
                                        save = true;
                                    }
                                }
                            }
                        }
                    }

                    configurationDatabase.DismountHive(save);
                }
            }
        }

        private void CleanupAllMountedDisks()
        {
            Logger.LogInfo("{0}: Cleaning up all mounted disks.", (object) MethodBase.GetCurrentMethod().Name);
            try
            {
                for (var index = 0; index < 10; ++index)
                    NativeImaging.DismountVirtualHardDisk((IntPtr) ServiceHandle, _storeId, true, false, false);
            }
            catch (ImageStorageException ex)
            {
            }
        }

        private void MountFullFlashImageStoreInternal(FullFlashUpdateImage.FullFlashUpdateStore store,
            PayloadReader payloadReader, StorePayload payload, bool randomizeGptIds)
        {
            var imageStyle = Image.ImageStyle;
            payloadReader.WriteToDisk(SafeStoreHandle, payload);
            using (var diskStreamSource = new DiskStreamSource(SafeStoreHandle, payload.StoreHeader.BytesPerBlock))
            {
                using (var dataBlockStream = new DataBlockStream(diskStreamSource, payload.StoreHeader.BytesPerBlock))
                {
                    if ((int) imageStyle == (int) ImageConstants.PartitionTypeGpt)
                        if (store.IsMainOSStore)
                        {
                            var guidPartitionTable = new GuidPartitionTable(
                                (int) NativeImaging.GetSectorSize((IntPtr) ServiceHandle, SafeStoreHandle), Logger);
                            guidPartitionTable.ReadFromStream(dataBlockStream, true);
                            guidPartitionTable.GetEntry(ImageConstants.MAINOS_PARTITION_NAME).Attributes |=
                                ImageConstants.GPT_ATTRIBUTE_NO_DRIVE_LETTER;
                            if (randomizeGptIds)
                                guidPartitionTable.RandomizeGptIds();
                            guidPartitionTable.FixCrcs();
                            guidPartitionTable.WriteToStream(dataBlockStream, true, false);
                            using (var virtualMemoryPtr = new VirtualMemoryPtr(payload.StoreHeader.BytesPerBlock))
                            {
                                foreach (var blockEntry in dataBlockStream.BlockEntries)
                                    if (blockEntry.DataSource.Source == DataBlockSource.DataSource.Memory)
                                    {
                                        var distanceToMove =
                                            blockEntry.BlockLocationsOnDisk[0].BlockIndex *
                                            (long) payload.StoreHeader.BytesPerBlock;
                                        long newFileLocation = 0;
                                        uint bytesWritten = 0;
                                        Marshal.Copy(blockEntry.DataSource.GetMemoryData(), 0,
                                            virtualMemoryPtr.AllocatedPointer, (int) payload.StoreHeader.BytesPerBlock);
                                        Win32Exports.SetFilePointerEx(SafeStoreHandle, distanceToMove,
                                            out newFileLocation, Win32Exports.MoveMethod.FILE_BEGIN);
                                        Win32Exports.WriteFile(SafeStoreHandle, virtualMemoryPtr.AllocatedPointer,
                                            payload.StoreHeader.BytesPerBlock, out bytesWritten);
                                    }
                            }
                        }
                }
            }

            NativeImaging.UpdateDiskLayout((IntPtr) ServiceHandle, SafeStoreHandle);
            _storeId = NativeImaging.GetDiskId((IntPtr) ServiceHandle, SafeStoreHandle);
            var partitionEntryList = new List<ImageStructures.PARTITION_ENTRY>();
            for (var index = 0; index < store.PartitionCount; ++index)
            {
                var partition = store.Partitions[index];
                var partitionEntry = new ImageStructures.PARTITION_ENTRY();
                PreparePartitionEntry(ref partitionEntry, store, partition, imageStyle, 1U);
                partitionEntryList.Add(partitionEntry);
            }

            partitionEntryList.TrimExcess();
            NativeImaging.WaitForPartitions((IntPtr) ServiceHandle, _storeId, partitionEntryList.ToArray());
        }

        private void ValidatePartitionStrings(FullFlashUpdateImage.FullFlashUpdatePartition partition)
        {
            if (partition.Name.Length > 32)
                throw new ImageStorageException(string.Format("The partition name is too long: {0}.", partition.Name));
            if (!string.IsNullOrEmpty(partition.FileSystem) && partition.FileSystem.Length > 32)
                throw new ImageStorageException(string.Format("Partition {0}'s file system is too long.",
                    partition.Name));
        }

        private ulong FlagsFromPartition(FullFlashUpdateImage.FullFlashUpdatePartition partition)
        {
            ulong num = 0;
            if (partition.Hidden)
                num |= 4611686018427387904UL;
            if (partition.ReadOnly)
                num |= 1152921504606846976UL;
            if (!partition.AttachDriveLetter)
                num |= 9223372036854775808UL;
            return num;
        }

        private void PreparePartitionEntry(ref ImageStructures.PARTITION_ENTRY partitionEntry,
            FullFlashUpdateImage.FullFlashUpdateStore store, FullFlashUpdateImage.FullFlashUpdatePartition partition,
            uint partitionStyle, uint alignmentInBytes)
        {
            if ((int) partitionStyle == (int) ImageConstants.PartitionTypeGpt)
            {
                Guid guid1;
                try
                {
                    guid1 = new Guid(partition.PartitionType);
                }
                catch (Exception ex)
                {
                    throw new ImageStorageException(string.Format("Partition {0}'s TYPE is invalid: {1}: {2}",
                        partition.Name, partition.PartitionType, ex.Message));
                }

                partitionEntry.PartitionType = guid1;
                var guid2 = Guid.Empty;
                var flag = false;
                if (!string.IsNullOrEmpty(partition.PartitionId))
                {
                    try
                    {
                        guid2 = new Guid(partition.PartitionId);
                    }
                    catch (Exception ex)
                    {
                        throw new ImageStorageException(string.Format("Partition {0}'s ID is invalid: {1}: {2}",
                            partition.Name, partition.PartitionId, ex.Message));
                    }

                    flag = true;
                }

                if (_manager.RandomizePartitionIDs)
                {
                    partitionEntry.PartitionId = Guid.NewGuid();
                }
                else if (string.Compare(ImageConstants.MAINOS_PARTITION_NAME, partition.Name, true,
                             CultureInfo.InvariantCulture) == 0)
                {
                    partitionEntry.PartitionId = ImageConstants.MAINOS_PARTITION_ID;
                    if (flag)
                        throw new ImageStorageException(string.Format(
                            "Unable to override protected partition {0}'s ID with {1}", partition.Name,
                            partition.PartitionId));
                }
                else if (string.Compare(ImageConstants.SYSTEM_PARTITION_NAME, partition.Name, true,
                             CultureInfo.InvariantCulture) == 0)
                {
                    partitionEntry.PartitionId = ImageConstants.SYSTEM_PARTITION_ID;
                    if (flag)
                        throw new ImageStorageException(string.Format(
                            "Unable to override protected partition {0}'s ID with {1}", partition.Name,
                            partition.PartitionId));
                }
                else if (string.Compare(ImageConstants.MMOS_PARTITION_NAME, partition.Name, true,
                             CultureInfo.InvariantCulture) == 0)
                {
                    partitionEntry.PartitionId = ImageConstants.MMOS_PARTITION_ID;
                    if (flag)
                        throw new ImageStorageException(string.Format(
                            "Unable to override protected partition {0}'s ID with {1}", partition.Name,
                            partition.PartitionId));
                }
                else
                {
                    partitionEntry.PartitionId = flag ? guid2 : Guid.NewGuid();
                }

                partitionEntry.PartitionFlags = FlagsFromPartition(partition);
            }
            else
            {
                if (partition.Bootable)
                    partitionEntry.MBRFlags = 128;
                var partitionType = partition.PartitionType;
                byte result = 0;
                if (partitionType.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!byte.TryParse(partitionType.Substring(2, partitionType.Length - 2), NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture, out result))
                        throw new ImageStorageException(
                            string.Format("Partition MBR style {0}'s type cannot be parsed.", partition.Name));
                }
                else if (!byte.TryParse(partitionType, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                {
                    throw new ImageStorageException(string.Format("Partition GPT style {0}'s type cannot be parsed.",
                        partition.Name));
                }

                partitionEntry.MBRType = result;
            }

            partitionEntry.FileSystem =
                !string.IsNullOrEmpty(partition.FileSystem) ? partition.FileSystem : string.Empty;
            partitionEntry.PartitionName = partition.Name;
            partitionEntry.AlignmentSizeInBytes = alignmentInBytes;
            partitionEntry.ClusterSize = partition.ClusterSize;
            partitionEntry.SectorCount = !partition.UseAllSpace
                ? (int) store.SectorSize == (int) VirtualHardDiskSectorSize ? partition.TotalSectors :
                partition.TotalSectors * (store.SectorSize / VirtualHardDiskSectorSize)
                : uint.MaxValue;
            partitionEntry.OffsetInSectors = partition.OffsetInSectors;
        }

        private bool IsImageCompressed(string accessPath)
        {
            var handle1 = IntPtr.Zero;
            var handle2 = IntPtr.Zero;
            try
            {
                handle1 = OfflineRegUtils.OpenHive(Path.Combine(accessPath, "Windows\\system32\\config\\SYSTEM"));
                handle2 = OfflineRegUtils.OpenKey(handle1, "Setup");
                return BitConverter.ToUInt32(OfflineRegUtils.GetValue(handle2, "Compact"), 0) == 1U;
            }
            catch (Win32Exception ex)
            {
            }
            catch (Exception ex)
            {
                Logger.LogWarning(string.Format("{0}: Unable to get Compact regkey value.",
                    MethodBase.GetCurrentMethod().Name));
            }
            finally
            {
                if (handle2 != IntPtr.Zero)
                {
                    OfflineRegUtils.CloseKey(handle2);
                    var zero = IntPtr.Zero;
                }

                if (handle1 != IntPtr.Zero)
                {
                    OfflineRegUtils.CloseHive(handle1);
                    var zero = IntPtr.Zero;
                }
            }

            return false;
        }

        private static bool IsPartitionHidden(FullFlashUpdateImage.FullFlashUpdatePartition partition)
        {
            Guid result;
            return partition.Hidden || Guid.TryParse(partition.PartitionType, out result) &&
                   result == ImageConstants.PARTITION_SYSTEM_GUID;
        }

        [Conditional("DEBUG")]
        internal void TestMountVirtualDisk(string existingDisk)
        {
            var tempFileName = Path.GetTempFileName();
            File.Copy(existingDisk, tempFileName, true);
            MountExistingVirtualHardDisk(tempFileName, true);
        }

        [Conditional("DEBUG")]
        internal void TestDismountVirtualDisk()
        {
            if (SafeStoreHandle.IsInvalid)
                return;
            DismountVirtualHardDisk(false);
        }

        [Conditional("DEBUG")]
        internal void TestValidateFileBuffer(byte[] fileBuffer, ulong diskOffset)
        {
            using (var virtualMemoryPtr = new VirtualMemoryPtr((uint) fileBuffer.Length))
            {
                uint bytesRead = 0;
                long newFileLocation = 0;
                Win32Exports.SetFilePointerEx(SafeStoreHandle, (long) diskOffset, out newFileLocation,
                    Win32Exports.MoveMethod.FILE_BEGIN);
                Win32Exports.ReadFile(SafeStoreHandle, (IntPtr) virtualMemoryPtr, virtualMemoryPtr.MemorySize,
                    out bytesRead);
                if (Win32Exports.memcmp(fileBuffer, (IntPtr) virtualMemoryPtr, (UIntPtr) bytesRead) != 0)
                    throw new ImageStorageException(string.Format("TEST: ValidateFileBuffer failed at disk offset {0}",
                        diskOffset));
            }
        }

        private class PartitionInfo
        {
            public byte MbrType { get; set; }

            public Guid GptType { get; set; }

            public byte MbrAttributes { get; set; }

            public ulong GptAttributes { get; set; }
        }
    }
}