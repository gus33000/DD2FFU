// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.ImageStorageManager
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{

    public class ImageStorageManager
    {
        private readonly IList<string> _partitionsTargeted;
        private readonly bool _recovery;
        private readonly Dictionary<FullFlashUpdateImage.FullFlashUpdateStore, ImageStorage> _storages;
        private uint _virtualHardDiskSectorSize;

        public ImageStorageManager()
            : this(new IULogger())
        {
        }

        public ImageStorageManager(IULogger logger)
            : this(logger, null, false)
        {
        }

        public ImageStorageManager(IULogger logger, IList<string> partitionsTargeted, bool recovery)
        {
            Logger = logger;
            _partitionsTargeted = partitionsTargeted;
            _recovery = recovery;
            _storages = [];
            _virtualHardDiskSectorSize = ImageConstants.DefaultVirtualHardDiskSectorSize;
            MountManagerScrubRegistry();
        }

        public IULogger Logger
        {
            get;
        }

        public FullFlashUpdateImage Image
        {
            get; private set;
        }

        public ReadOnlyCollection<ImageStorage> Storages => _storages.Values.ToList().AsReadOnly();

        public ImageStorage MainOSStorage => _storages[_storages.Keys.Single(s => s.IsMainOSStore)];

        public uint VirtualHardDiskSectorSize
        {
            get => _virtualHardDiskSectorSize;
            set
            {
                _virtualHardDiskSectorSize = value;
                foreach (ImageStorage imageStorage in _storages.Values)
                {
                    imageStorage.VirtualHardDiskSectorSize = value;
                }
            }
        }

        public bool RandomizeDiskIds
        {
            get; set;
        }

        public bool RandomizePartitionIDs
        {
            get; set;
        }

        internal uint BytesPerBlock => ImageConstants.PAYLOAD_BLOCK_SIZE;

        public void SetFullFlashImage(FullFlashUpdateImage image)
        {
            Image = image;
            foreach (ImageStorage imageStorage in _storages.Values)
            {
                imageStorage.SetFullFlashImage(image);
            }
        }

        public void CreateFullFlashImage(FullFlashUpdateImage image)
        {
            if (Image != null)
            {
                DismountFullFlashImage(false);
            }

            int tickCount = Environment.TickCount;
            image.DisplayImageInformation(Logger);
            CheckForDuplicateNames(image);
            ValidateMainOsInImage(image);
            if (image.DefaultPartitionAlignmentInBytes < ImageConstants.PAYLOAD_BLOCK_SIZE)
            {
                image.DefaultPartitionAlignmentInBytes = ImageConstants.PAYLOAD_BLOCK_SIZE;
            }

            ImageStructures.STORE_ID[] storeIds = new ImageStructures.STORE_ID[image.Stores.Count()];
            for (int index = 0; index < image.Stores.Count(); ++index)
            {
                ImageStructures.STORE_ID storeId = new();
                FullFlashUpdateImage.FullFlashUpdateStore store = image.Stores[index];
                storeId.StoreType = image.ImageStyle;
                if (RandomizeDiskIds)
                {
                    storeId.StoreId_GPT = Guid.NewGuid();
                }
                else if ((int)storeId.StoreType == (int)ImageConstants.PartitionTypeGpt)
                {
                    storeId.StoreId_GPT = store.IsMainOSStore ? ImageConstants.SYSTEM_STORE_GUID : Guid.Parse(store.Id);
                }
                else
                {
                    storeId.StoreId_MBR = store.IsMainOSStore
                                        ? ImageConstants.SYSTEM_STORE_SIGNATURE
                                        : Convert.ToUInt32(store.Id);
                }

                storeIds[index] = storeId;
            }

            for (int index1 = 0; index1 < image.Stores.Count(); ++index1)
            {
                FullFlashUpdateImage.FullFlashUpdateStore store = image.Stores[index1];
                if (store.SectorSize > BytesPerBlock)
                {
                    throw new ImageStorageException(string.Format(
                                        "The sector size (0x{0:x} bytes) is greater than the image block size (0x{1x} bytes)",
                                        store.SectorSize, BytesPerBlock));
                }

                if (BytesPerBlock % store.SectorSize != 0U)
                {
                    throw new ImageStorageException(string.Format(
                                        "The block size (0x{0:x} bytes) is not a mulitple of the sector size (0x{1x} bytes)",
                                        BytesPerBlock, store.SectorSize));
                }

                long sectorCount = store.SectorCount;
                if (sectorCount == 0L)
                {
                    throw new ImageStorageException(
                                        "Please specify an image size using the MinSectorCount field in the device platform information file.");
                }

                if ((ulong)sectorCount * store.SectorSize % BytesPerBlock != 0UL)
                {
                    throw new ImageStorageException(string.Format(
                                        "The image size, specified by MinSectorCount, needs to be a multiple of {0} (0x{0:x}) sectors.",
                                        BytesPerBlock / store.SectorSize));
                }

                for (int index2 = 0; index2 < store.Partitions.Count; ++index2)
                {
                    foreach (FullFlashUpdateImage.FullFlashUpdatePartition partition in store.Partitions)
                    {
                        if (partition.ByteAlignment > 0U && partition.ByteAlignment < ImageConstants.PAYLOAD_BLOCK_SIZE)
                        {
                            partition.ByteAlignment = ImageConstants.PAYLOAD_BLOCK_SIZE;
                        }
                    }
                }

                _ = CreateVirtualHardDisk(store, null, image.ImageStyle, true, storeIds[index1], storeIds);
            }

            Logger.LogInfo("Storage Service: Created a new image in {0:F1} seconds.",
                (Environment.TickCount - tickCount) / 1000.0);
            Image = image;
        }

        public uint MountFullFlashImage(FullFlashUpdateImage image, bool randomizeGptIds)
        {
            if (Image != null)
            {
                DismountFullFlashImage(false);
            }

            uint num = 1;
            using (FileStream imageStream = image.GetImageStream())
            {
                PayloadReader payloadReader = new(imageStream);
                if (payloadReader.Payloads.Count() != image.StoreCount)
                {
                    throw new ImageStorageException("Store counts in metadata and store header do not match");
                }

                for (int index = 0; index < image.StoreCount; ++index)
                {
                    FullFlashUpdateImage.FullFlashUpdateStore store = image.Stores[index];
                    StorePayload payload = payloadReader.Payloads[index];
                    payloadReader.ValidatePayloadPartitions((int)store.SectorSize,
                        store.SectorCount * (long)store.SectorSize, payload, image.ImageStyle, store.IsMainOSStore,
                        Logger);
                    ImageStorage imageStorage = new(Logger, this)
                    {
                        VirtualHardDiskSectorSize = VirtualHardDiskSectorSize
                    };
                    imageStorage.MountFullFlashImageStore(store, payloadReader, payload, randomizeGptIds);
                    _storages.Add(store, imageStorage);
                    num = payload.StoreHeader.MajorVersion;
                }
            }

            Image = image;
            return num;
        }

        public void DismountFullFlashImage(bool saveChanges)
        {
            OutputWrapper outputWrapper = null;
            if (Image == null)
            {
                return;
            }

            if (_storages.Count == 0)
            {
                return;
            }

            try
            {
                outputWrapper = new OutputWrapper(Image.Stores[0].BackingFile);
                DismountFullFlashImage(saveChanges, outputWrapper, true, 1U);
            }
            finally
            {
                outputWrapper?.FinalizeWrapper();
            }
        }

        public void DismountFullFlashImage(bool saveChanges, IPayloadWrapper payloadWrapper)
        {
            DismountFullFlashImage(saveChanges, payloadWrapper, true);
        }

        public void DismountFullFlashImage(bool saveChanges, IPayloadWrapper payloadWrapper, bool deleteFile)
        {
            DismountFullFlashImage(saveChanges, payloadWrapper, deleteFile, 1U);
        }

        public void DismountFullFlashImage(bool saveChanges, IPayloadWrapper payloadWrapper, bool deleteFile,
            uint storeHeaderVersion)
        {
            int tickCount = Environment.TickCount;
            if (Image == null & saveChanges)
            {
                throw new ImageStorageException(string.Format(
                                "{0}: Cannot save changes because the full flash update image is null.",
                                MethodBase.GetCurrentMethod().Name));
            }

            if (_storages.Keys.Count(s => s.IsMainOSStore) != 1)
            {
                throw new ImageStorageException(string.Format(
                                "{0}: One and only one storage can be the MainOS storage.", MethodBase.GetCurrentMethod().Name));
            }

            foreach (ImageStorage imageStorage in _storages.Values)
            {
                if (imageStorage.SafeStoreHandle == null)
                {
                    Logger.DebugLogger("{0}: This function was called when no image is mounted.",
                        MethodBase.GetCurrentMethod().Name);
                    return;
                }

                if (imageStorage.SafeStoreHandle.IsInvalid)
                {
                    throw new ImageStorageException(string.Format(
                                        "{0}: This function was called without a mounted image.", MethodBase.GetCurrentMethod().Name));
                }

                if (imageStorage.Image == null & saveChanges)
                {
                    throw new ImageStorageException(string.Format(
                                        "{0}: Cannot save changes because the full flash update image is null.",
                                        MethodBase.GetCurrentMethod().Name));
                }
            }

            if (saveChanges)
            {
                foreach (ImageStorage imageStorage in _storages.Values)
                {
                    if ((int)Image.ImageStyle == (int)ImageConstants.PartitionTypeMbr)
                    {
                        Logger.LogInfo("{0}:[{1}] Updating the BCD to fix partition offsets.",
                            MethodBase.GetCurrentMethod().Name,
                            (Environment.TickCount - tickCount) / 1000.0);
                        imageStorage.UpdateBootConfigurationDatabase(
                            Image.UEFI ? ImageConstants.EFI_BCD_FILE_PATH : ImageConstants.BCD_FILE_PATH,
                            ImageConstants.SYSTEM_STORE_SIGNATURE);
                    }

                    if (imageStorage.IsMainOSStorage)
                    {
                        try
                        {
                            string partitionPath = GetPartitionPath("CrashDump");
                            if (!string.IsNullOrEmpty(partitionPath))
                            {
                                if (string.Compare("NTFS", GetPartitionFileSystem("CrashDump"), true,
                                                                    CultureInfo.InvariantCulture) != 0)
                                {
                                    using (FileStream fileStream = File.Create(Path.Combine(partitionPath, "10779436.sys")))
                                    {
                                        StreamWriter streamWriter = new(fileStream);
                                        streamWriter.WriteLine("10779436");
                                        streamWriter.Flush();
                                    }

                                    File.SetAttributes(Path.Combine(partitionPath, "10779436.sys"),
                                        FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.Archive);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }

                        Logger.LogInfo("{0}:[{1}] Enabling USN journal on partition {2}.",
                            MethodBase.GetCurrentMethod().Name,
                            (Environment.TickCount - tickCount) / 1000.0,
                            ImageConstants.MAINOS_PARTITION_NAME);
                        CreateUsnJournal(ImageConstants.MAINOS_PARTITION_NAME);
                        Logger.LogInfo("{0}:[{1}] Enabling USN journal on partition {2}.",
                            MethodBase.GetCurrentMethod().Name,
                            (Environment.TickCount - tickCount) / 1000.0,
                            ImageConstants.DATA_PARTITION_NAME);
                        CreateUsnJournal(ImageConstants.DATA_PARTITION_NAME);
                    }

                    if (imageStorage.IsMainOSStorage)
                    {
                        NativeImaging.WriteMountManagerRegistry2((nint)imageStorage.ServiceHandle,
                            imageStorage.StoreId, true);
                        NativeImaging.NormalizeVolumeMountPoints((nint)imageStorage.ServiceHandle,
                            imageStorage.StoreId, GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME));
                    }
                }

                Logger.LogInfo("{0}:[{1}] Flushing all volumes.", MethodBase.GetCurrentMethod().Name,
                    (Environment.TickCount - tickCount) / 1000.0);
                FlushVolumesForDismount();
                using (VirtualDiskPayloadManager diskPayloadManager = new(Logger, (ushort)storeHeaderVersion,
                    (ushort)_storages.Count(), _recovery))
                {
                    foreach (ImageStorage storage in _storages.Values)
                    {
                        diskPayloadManager.AddStore(storage);
                    }

                    diskPayloadManager.Write(payloadWrapper);
                }

                foreach (ImageStorage imageStorage in _storages.Values)
                {
                    imageStorage.SafeStoreHandle.Close();
                    Logger.LogInfo("{0}:[{1}] Final VHD dismount.", MethodBase.GetCurrentMethod().Name,
                        (Environment.TickCount - tickCount) / 1000.0);
                    NativeImaging.DismountVirtualHardDisk((nint)imageStorage.ServiceHandle, imageStorage.StoreId,
                        true, deleteFile, false);
                    Logger.LogInfo("{0}:[{1}] Cleaning up temporary paths.",
                        MethodBase.GetCurrentMethod().Name,
                        (Environment.TickCount - tickCount) / 1000.0);
                    imageStorage.Cleanup();
                }
            }
            else
            {
                foreach (ImageStorage imageStorage in _storages.Values)
                {
                    imageStorage.SafeStoreHandle?.Close();
                    NativeImaging.DismountVirtualHardDiskByName((nint)imageStorage.ServiceHandle,
                        imageStorage.VirtualDiskFilePath, deleteFile);
                    Logger.LogInfo("{0}:[{1}] Cleaning up temporary paths.",
                        MethodBase.GetCurrentMethod().Name,
                        (Environment.TickCount - tickCount) / 1000.0);
                    imageStorage.Cleanup();
                }
            }

            Logger.LogInfo("Storage Service: Dismounting the image in {0:F1} seconds.",
                (Environment.TickCount - tickCount) / 1000.0);
            Image = null;
            _storages.Clear();
        }

        public string CreateVirtualHardDisk(FullFlashUpdateImage.FullFlashUpdateStore store, string imagePath,
            uint partitionStyle, bool preparePartitions)
        {
            ImageStructures.STORE_ID storeId = new()
            {
                StoreType = partitionStyle
            };
            if (RandomizeDiskIds)
            {
                storeId.StoreId_GPT = Guid.NewGuid();
            }
            else if ((int)partitionStyle == (int)ImageConstants.PartitionTypeGpt)
            {
                storeId.StoreId_GPT = store.IsMainOSStore ? ImageConstants.SYSTEM_STORE_GUID : Guid.Parse(store.Id);
            }
            else
            {
                storeId.StoreId_MBR = store.IsMainOSStore
                                ? ImageConstants.SYSTEM_STORE_SIGNATURE
                                : Convert.ToUInt32(store.Id);
            }

            return CreateVirtualHardDisk(store, imagePath, partitionStyle, (preparePartitions ? 1 : 0) != 0, storeId,
                            new ImageStructures.STORE_ID[1]
                            {
                    storeId
                            });
        }

        public string CreateVirtualHardDisk(FullFlashUpdateImage.FullFlashUpdateStore store, string imagePath,
            uint partitionStyle, bool preparePartitions, ImageStructures.STORE_ID storeId,
            ImageStructures.STORE_ID[] storeIds)
        {
            if (Image != null)
            {
                DismountFullFlashImage(false);
            }

            ImageStorage imageStorage = new(Logger, this, storeId)
            {
                VirtualHardDiskSectorSize = VirtualHardDiskSectorSize
            };
            imageStorage.CreateVirtualHardDiskFromStore(store, imagePath, partitionStyle, preparePartitions, storeIds);
            _storages.Add(store, imageStorage);
            return imageStorage.VirtualDiskFilePath;
        }

        public void MountExistingVirtualHardDisk(string imagePath, bool readOnly)
        {
            ImageStorage storage = new(Logger, this)
            {
                VirtualHardDiskSectorSize = VirtualHardDiskSectorSize
            };
            storage.MountExistingVirtualHardDisk(imagePath, readOnly);
            _ = CreateFullFlashObjectFromAttachedImage(storage);
        }

        public void DismountVirtualHardDisk()
        {
            DismountVirtualHardDisk(false, false, false);
        }

        public void DismountVirtualHardDisk(bool skipPostProcessing, bool deleteFile)
        {
            DismountVirtualHardDisk(skipPostProcessing, deleteFile, false);
        }

        public void DismountVirtualHardDisk(bool skipPostProcessing, bool deleteFile, bool normalizeDiskSignature)
        {
            foreach (ImageStorage imageStorage in _storages.Values)
            {
                imageStorage.DismountVirtualHardDisk(skipPostProcessing, normalizeDiskSignature);
                if (deleteFile)
                {
                    LongPathFile.Delete(imageStorage.VirtualDiskFilePath);
                }
            }

            _storages.Clear();
        }

        public FullFlashUpdateImage CreateFullFlashObjectFromAttachedImage(ImageStorage storage)
        {
            return CreateFullFlashObjectFromAttachedImage(
            [
                storage
            ]);
        }

        public FullFlashUpdateImage CreateFullFlashObjectFromAttachedImage(List<ImageStorage> storages)
        {
            string deviceLayoutPath;
            string platformInfoPath;
            try
            {
                ImageStorage imageStorage = storages.Single(s => s.IsMainOSStorage);
                deviceLayoutPath = Path.Combine(imageStorage.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME),
                    DevicePaths.DeviceLayoutFilePath);
                platformInfoPath = Path.Combine(imageStorage.GetPartitionPath(ImageConstants.MAINOS_PARTITION_NAME),
                    DevicePaths.OemDevicePlatformFilePath);
            }
            catch (Exception)
            {
                throw new ImageStorageException("Unable to find MainOS store or there are more than one.");
            }

            return CreateFullFlashObjectFromAttachedImage(storages, deviceLayoutPath, platformInfoPath);
        }

        public FullFlashUpdateImage CreateFullFlashObjectFromAttachedImage(List<ImageStorage> storages,
            string deviceLayoutPath, string platformInfoPath)
        {
            ImageGenerator imageGenerator = new();
            ImageGeneratorParameters parameters = new();
            try
            {
                parameters.Initialize(Logger);
                parameters.ProcessInputXML(deviceLayoutPath, platformInfoPath);
                for (int index = 0; index < parameters.Stores.Count; ++index)
                {
                    InputStore store = parameters.Stores[index];
                    ImageStorage storage = storages[index];
                    storage.VirtualHardDiskSectorSize = parameters.VirtualHardDiskSectorSize;
                    foreach (InputPartition partition in store.Partitions)
                    {
                        if (partition.MinFreeSectors != 0U)
                        {
                            partition.TotalSectors = (uint)storage.GetPartitionSize(partition.Name);
                        }
                    }
                }

                imageGenerator.Initialize(parameters, Logger);
                FullFlashUpdateImage ffu = imageGenerator.CreateFFU();
                if (storages.Count != ffu.StoreCount)
                {
                    throw new ImageStorageException(
                                        "Number of ImageStorage objects and stores in device layout do not match");
                }

                for (int index = 0; index < storages.Count; ++index)
                {
                    storages[index].SetFullFlashUpdateStore(ffu.Stores[index]);
                    _storages.Add(ffu.Stores[index], storages[index]);
                }

                Image = ffu;
            }
            catch (Exception ex)
            {
                throw new ImageStorageException("Unable to create a FullFlashImage object.", ex);
            }

            return Image;
        }

        public ImageStorage AttachToMountedVirtualHardDisk(string physicalDiskPath, bool readOnly)
        {
            return AttachToMountedVirtualHardDisk(physicalDiskPath, readOnly, true);
        }

        public ImageStorage AttachToMountedVirtualHardDisk(string physicalDiskPath, bool readOnly, bool isMainOSStore)
        {
            ImageStorage imageStorage = new(Logger, this)
            {
                VirtualHardDiskSectorSize = VirtualHardDiskSectorSize
            };
            imageStorage.AttachToMountedVirtualHardDisk(physicalDiskPath, readOnly, isMainOSStore);
            return imageStorage;
        }

        public void DetachVirtualHardDisk(bool deleteFile)
        {
            foreach (ImageStorage imageStorage in _storages.Values)
            {
                imageStorage.DetachVirtualHardDisk(deleteFile);
            }

            _storages.Clear();
        }

        public ImageStorage GetImageStorage(FullFlashUpdateImage.FullFlashUpdateStore store)
        {
            return _storages[store];
        }

        public bool IsPartitionTargeted(string partition)
        {
            return _partitionsTargeted == null
|| _partitionsTargeted.Any(p => string.Compare(partition, p, true, CultureInfo.InvariantCulture) == 0);
        }

        private static void CheckForDuplicateNames(FullFlashUpdateImage image)
        {
            List<string> stringList = [];
            foreach (FullFlashUpdateImage.FullFlashUpdateStore store in image.Stores)
            {
                foreach (FullFlashUpdateImage.FullFlashUpdatePartition partition in store.Partitions)
                {
                    if (stringList.Contains(partition.Name))
                    {
                        throw new ImageStorageException(string.Format("Partition {0} is included more than once.",
                                                partition.Name));
                    }

                    stringList.Add(partition.Name);
                }
            }
        }

        private static void ValidateMainOsInImage(FullFlashUpdateImage image)
        {
            bool flag = false;
            foreach (FullFlashUpdateImage.FullFlashUpdatePartition partition in image.Stores.Single(s => s.IsMainOSStore).Partitions)
            {
                if (string.Compare(partition.Name, ImageConstants.MAINOS_PARTITION_NAME, true,
                                    CultureInfo.InvariantCulture) == 0)
                {
                    if (string.IsNullOrEmpty(partition.FileSystem))
                    {
                        throw new ImageStorageException(string.Format(
                                                "{0}: Partition '{1}' must have a valid file system.", MethodBase.GetCurrentMethod().Name,
                                                ImageConstants.MAINOS_PARTITION_NAME));
                    }

                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                throw new ImageStorageException(string.Format(
                                "{0}: The full flash update image must contain a partition '{1}'.",
                                MethodBase.GetCurrentMethod().Name, ImageConstants.MAINOS_PARTITION_NAME));
            }
        }

        public Guid GetPartitionTypeGpt(string partitionName)
        {
            return GetImageStorageByPartitionName(partitionName).GetPartitionTypeGpt(partitionName);
        }

        public byte GetPartitionTypeMbr(string partitionName)
        {
            return GetImageStorageByPartitionName(partitionName).GetPartitionTypeMbr(partitionName);
        }

        public string GetPartitionPath(string partitionName)
        {
            return GetImageStorageByPartitionName(partitionName).GetPartitionPath(partitionName);
        }

        public ulong GetPartitionSize(string partitionName)
        {
            return GetImageStorageByPartitionName(partitionName).GetPartitionSize(partitionName);
        }

        public void SetPartitionType(string partitionName, Guid partitionType)
        {
            GetImageStorageByPartitionName(partitionName).SetPartitionType(partitionName, partitionType);
        }

        public void SetPartitionType(string partitionName, byte partitionType)
        {
            GetImageStorageByPartitionName(partitionName).SetPartitionType(partitionName, partitionType);
        }

        public string GetPartitionFileSystem(string partitionName)
        {
            return GetImageStorageByPartitionName(partitionName).GetPartitionFileSystem(partitionName);
        }

        public bool PartitionIsMountedRaw(string partitionName)
        {
            return GetImageStorageByPartitionName(partitionName).PartitionIsMountedRaw(partitionName);
        }

        public void FormatPartition(string partitionName, string fileSsytem, uint cbClusterSize)
        {
            GetImageStorageByPartitionName(partitionName).FormatPartition(partitionName, fileSsytem, cbClusterSize);
        }

        public SafeFileHandle OpenVolumeHandle(string partitionName)
        {
            return GetImageStorageByPartitionName(partitionName).OpenVolumeHandle(partitionName);
        }

        public void WaitForVolume(string volumeName)
        {
            GetImageStorageByPartitionName(volumeName).WaitForVolume(volumeName);
        }

        public void FlushVolumesForDismount()
        {
            foreach (FullFlashUpdateImage.FullFlashUpdateStore key in _storages.Keys)
            {
                ImageStorage storage = _storages[key];
                foreach (FullFlashUpdateImage.FullFlashUpdatePartition partition in key.Partitions)
                {
                    storage.WaitForVolume(partition.Name);
                    if (!storage.PartitionIsMountedRaw(partition.Name))
                    {
                        using SafeVolumeHandle safeVolumeHandle = new(storage, partition.Name);
                        Win32Exports.FlushFileBuffers(safeVolumeHandle.VolumeHandle);
                    }
                }
            }
        }

        public ulong GetFreeBytesOnVolume(string partitionName)
        {
            return GetImageStorageByPartitionName(partitionName).GetFreeBytesOnVolume(partitionName);
        }

        public void CreateJunction(string sourceName, string targetPartition, string targetPath)
        {
            CreateJunction(sourceName, targetPartition, targetPath, false);
        }

        public void CreateJunction(string sourceName, string targetPartition, string targetPath, bool useWellKnownGuids)
        {
            GetImageStorageByPartitionName(targetPartition)
                .CreateJunction(sourceName, targetPartition, targetPath, useWellKnownGuids);
        }

        public void CreateUsnJournal(string partitionName)
        {
            GetImageStorageByPartitionName(partitionName).CreateUsnJournal(partitionName);
        }

        public void AttachWOFToVolume(string partitionName)
        {
            GetImageStorageByPartitionName(partitionName).AttachWOFToVolume(partitionName);
        }

        public void LockAndDismountVolume(string partitionName)
        {
            LockAndDismountVolume(partitionName, false);
        }

        public void LockAndDismountVolume(string partitionName, bool forceDismount)
        {
            GetImageStorageByPartitionName(partitionName).LockAndDismountVolume(partitionName, forceDismount);
        }

        public void UnlockVolume(string partitionName)
        {
            GetImageStorageByPartitionName(partitionName).UnlockVolume(partitionName);
        }

        private ImageStorage GetImageStorageByPartitionName(string partitionName)
        {
            return _storages[
                _storages.Keys.Single(s => s.Partitions.Exists(p =>
                    string.Compare(partitionName, p.Name, true, CultureInfo.InvariantCulture) == 0))];
        }

        private void MountManagerScrubRegistry()
        {
            using SafeFileHandle file = Win32Exports.CreateFile(Win32Exports.MountManagerPath,
                Win32Exports.DesiredAccess.GENERIC_READ | Win32Exports.DesiredAccess.GENERIC_WRITE,
                Win32Exports.ShareMode.FILE_SHARE_READ | Win32Exports.ShareMode.FILE_SHARE_WRITE,
                Win32Exports.CreationDisposition.OPEN_EXISTING, Win32Exports.FlagsAndAttributes.FILE_ATTRIBUTE_NORMAL);
            Win32Exports.DeviceIoControl(file.DangerousGetHandle(), 7192632U, null, 0, null, 0, out int bytesReturned);
        }
    }
}