// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.GptDataEntryGenerator
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DD2FFU;
using Microsoft.Win32.SafeHandles;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class GptDataEntryGenerator : IEntryGenerator
    {
        private readonly List<FullFlashUpdateImage.FullFlashUpdatePartition> _finalPartitions;
        private FullFlashUpdateImage _image;
        private readonly StorePayload _payload;
        private List<DataBlockEntry> _phase2PartitionTableEntries;
        private readonly bool _recovery;
        private readonly uint _sectorSize;
        private readonly SafeFileHandle _sourceHandle;
        private readonly ImageStorage _storage;
        private readonly ISourceAllocation _storageAllocation;
        private readonly GuidPartitionTable _table;

        public GptDataEntryGenerator(ImageStorage storage, StorePayload storePayload,
            ISourceAllocation storageAllocation, SafeFileHandle sourceHandle, uint sourceSectorSize, bool recovery)
        {
            _payload = storePayload;
            _storageAllocation = storageAllocation;
            _sourceHandle = sourceHandle;
            _image = storage.Image;
            _sectorSize = sourceSectorSize;
            _table = new GuidPartitionTable((int) sourceSectorSize, storage.Logger);
            _finalPartitions = new List<FullFlashUpdateImage.FullFlashUpdatePartition>();
            _storage = storage;
            _recovery = recovery;
            if (storePayload.StoreHeader.BytesPerBlock < _storage.Store.SectorSize)
                throw new ImageStorageException("The data block size is less than the device sector size.");
            if (storePayload.StoreHeader.BytesPerBlock % _storage.Store.SectorSize != 0U)
                throw new ImageStorageException("The data block size is not a multiple of the device sector size.");
            if (storageAllocation == null)
                return;
            if (storePayload.StoreHeader.BytesPerBlock > storageAllocation.GetAllocationSize())
                throw new ImageStorageException(
                    "The payload block size is larger than the allocation size of the temp store.");
            if (storageAllocation.GetAllocationSize() % storePayload.StoreHeader.BytesPerBlock != 0U)
                throw new ImageStorageException(
                    "The allocation size of the temp store is not a multiple of the payload block size.");
        }

        public void GenerateEntries(bool onlyAllocateDefinedGptEntries)
        {
            _payload.Phase1DataEntries = GeneratePhase1Entries();
            _payload.Phase2DataEntries = GeneratePhase2Entries(onlyAllocateDefinedGptEntries);
            if (_recovery)
                return;
            _payload.Phase3DataEntries = GeneratePhase3Entries(onlyAllocateDefinedGptEntries);
        }

        private List<DataBlockEntry> GeneratePhase1Entries()
        {
            var dataBlockEntryList = new List<DataBlockEntry>((int) (131072U / _payload.StoreHeader.BytesPerBlock));
            for (var index = 0; index < dataBlockEntryList.Capacity; ++index)
                dataBlockEntryList.Add(new DataBlockEntry(_payload.StoreHeader.BytesPerBlock)
                {
                    DataSource =
                    {
                        Source = DataBlockSource.DataSource.Zero
                    },
                    BlockLocationsOnDisk =
                    {
                        new DiskLocation((uint) index, DiskLocation.DiskAccessMethod.DiskBegin),
                        new DiskLocation((uint) index, DiskLocation.DiskAccessMethod.DiskEnd)
                    }
                });
            _payload.StoreHeader.InitialPartitionTableBlockCount = (uint) dataBlockEntryList.Count;
            return dataBlockEntryList;
        }

        private List<DataBlockEntry> GeneratePhase2Entries(bool onlyAllocateDefinedGptEntries)
        {
            var dataBlockStream =
                new DataBlockStream(new DiskStreamSource(_sourceHandle, _payload.StoreHeader.BytesPerBlock),
                    _payload.StoreHeader.BytesPerBlock);
            _table.ReadFromStream(dataBlockStream, true);
            _table.ValidateCrcs();
            for (var index = 0; index < _storage.Store.PartitionCount; ++index)
            {
                var partition = _storage.Store.Partitions[index];
                if (!partition.RequiredToFlash)
                {
                    _finalPartitions.Add(partition);
                    _table.RemoveEntry(partition.Name);
                }
            }

            if (_storage.IsMainOSStorage)
                _table.NormalizeGptIds(out FileSystemSourceAllocation.OriginalSystemPartition);
            _table.FixCrcs();
            _table.WriteToStream(dataBlockStream, true, onlyAllocateDefinedGptEntries);
            _phase2PartitionTableEntries = dataBlockStream.BlockEntries;
            var dataBlockEntryList = new List<DataBlockEntry>();
            foreach (var entry in _table.Entries)
                if (entry.StartingSector > 0UL)
                    GenerateDataEntries(dataBlockEntryList, entry);
            FilterPartitionTablesFromDataBlocks(dataBlockEntryList);
            _payload.StoreHeader.FlashOnlyPartitionTableBlockCount = (uint) _phase2PartitionTableEntries.Count;
            _payload.StoreHeader.FlashOnlyPartitionTableBlockIndex =
                _payload.StoreHeader.InitialPartitionTableBlockCount + (uint) dataBlockEntryList.Count;
            dataBlockEntryList.AddRange(_phase2PartitionTableEntries);
            return dataBlockEntryList;
        }

        private List<DataBlockEntry> GeneratePhase3Entries(bool onlyAllocateDefinedGptEntries)
        {
            var dataBlockEntryList = new List<DataBlockEntry>();
            var diskStreamSource = new DiskStreamSource(_sourceHandle, _payload.StoreHeader.BytesPerBlock);
            var dataBlockStream1 = new DataBlockStream(diskStreamSource, _payload.StoreHeader.BytesPerBlock);
            _table.ReadFromStream(dataBlockStream1, true);
            _table.ValidateCrcs();
            while (_finalPartitions.Count > 0)
                foreach (var entry in _table.Entries)
                    if (string.Compare(entry.PartitionName, _finalPartitions[0].Name, true,
                            CultureInfo.InvariantCulture) == 0)
                    {
                        GenerateDataEntries(dataBlockEntryList, entry);
                        _finalPartitions.RemoveAt(0);
                        break;
                    }

            var entry1 = _table.GetEntry(ImageConstants.MAINOS_PARTITION_NAME);
            if (entry1 != null)
                entry1.Attributes &= ~ImageConstants.GPT_ATTRIBUTE_NO_DRIVE_LETTER;
            if (_storage.IsMainOSStorage)
                _table.NormalizeGptIds(out FileSystemSourceAllocation.OriginalSystemPartition);
            _table.FixCrcs();
            _table.WriteToStream(dataBlockStream1, true, onlyAllocateDefinedGptEntries);
            FilterPartitionTablesFromDataBlocks(dataBlockEntryList);
            if (!_recovery)
            {
                _payload.StoreHeader.FinalPartitionTableBlockCount = (uint) dataBlockStream1.BlockEntries.Count;
                _payload.StoreHeader.FinalPartitionTableBlockIndex =
                    (uint) ((int) _payload.StoreHeader.FlashOnlyPartitionTableBlockIndex +
                            (int) _payload.StoreHeader.FlashOnlyPartitionTableBlockCount + dataBlockEntryList.Count);
            }
            else
            {
                _payload.StoreHeader.FinalPartitionTableBlockCount = 0U;
                _payload.StoreHeader.FinalPartitionTableBlockIndex =
                    _payload.StoreHeader.FlashOnlyPartitionTableBlockIndex +
                    _payload.StoreHeader.FlashOnlyPartitionTableBlockCount;
            }

            dataBlockEntryList.AddRange(dataBlockStream1.BlockEntries);
            var dataBlockStream2 = new DataBlockStream(diskStreamSource, _payload.StoreHeader.BytesPerBlock);
            _table.ReadFromStream(dataBlockStream2, false);
            var blockEntries = dataBlockStream2.BlockEntries;
            _payload.StoreHeader.FinalPartitionTableBlockCount += (uint) dataBlockStream2.BlockEntries.Count;
            ConvertEntriesToUseEndOfDisk(blockEntries, NativeImaging.GetSectorCount(IntPtr.Zero, _sourceHandle));
            dataBlockEntryList.AddRange(blockEntries);
            return dataBlockEntryList;
        }

        private void GenerateDataEntries(List<DataBlockEntry> dataEntries, GuidPartitionTableEntry entry)
        {
            List<DataBlockEntry> dataBlocks;
            if (!_storage.PartitionIsMountedRaw(entry.PartitionName))
            {
                var partitionOffset =
                    NativeImaging.GetPartitionOffset((IntPtr) _storage.ServiceHandle, _storage.StoreId,
                        entry.PartitionName) * _sectorSize;
                dataBlocks = new FileSystemSourceAllocation(_storage, entry.PartitionName, partitionOffset,
                    _payload.StoreHeader.BytesPerBlock).GenerateDataEntries();
            }
            else if (!_storage.IsBackingFileVhdx() || _storage.IsPartitionTargeted(entry.PartitionName))
            {
                var num = (entry.LastSector + 1UL - entry.StartingSector) * _sectorSize;
                dataBlocks = GenerateDataEntriesFromDisk((long) entry.StartingSector * _sectorSize, (long) num);
                FilterUnAllocatedDataEntries(dataBlocks);
            }
            else
            {
                dataBlocks = new List<DataBlockEntry>();
            }

            dataEntries.AddRange(dataBlocks);
        }

        private void FilterPartitionTablesFromDataBlocks(List<DataBlockEntry> dataBlocks)
        {
            var bytesPerBlock = (int) _payload.StoreHeader.BytesPerBlock;
            foreach (var dataBlock in dataBlocks)
            foreach (var partitionTableEntry in _phase2PartitionTableEntries)
                if ((int) dataBlock.BlockLocationsOnDisk[0].BlockIndex ==
                    (int) partitionTableEntry.BlockLocationsOnDisk[0].BlockIndex)
                {
                    dataBlock.DataSource = new DataBlockSource();
                    dataBlock.DataSource.Source = partitionTableEntry.DataSource.Source;
                    dataBlock.DataSource.SetMemoryData(partitionTableEntry.DataSource.GetMemoryData(), 0,
                        bytesPerBlock);
                    break;
                }
        }

        private void FilterUnAllocatedDataEntries(List<DataBlockEntry> dataBlocks)
        {
            var bytesPerBlock = (int) _payload.StoreHeader.BytesPerBlock;
            for (var index = 0; index < dataBlocks.Count; ++index)
            {
                var dataBlock = dataBlocks[index];

                var parts = _table.Entries;

                if (dataBlock.DataSource.Source == DataBlockSource.DataSource.Disk && _storageAllocation != null &&
                    !_storageAllocation.BlockIsAllocated(dataBlock.DataSource.StorageOffset) || parts.Any(x => Constants.partitions.Any(y => y.ToLower() == x.PartitionName.ToLower() && x.StartingSector * 0x200 <= dataBlock.DataSource.StorageOffset && dataBlock.DataSource.StorageOffset <= x.LastSector * 0x200)))
                {
                    dataBlocks.RemoveAt(index--);
                }
            }
        }

        private List<DataBlockEntry> GenerateDataEntriesFromDisk(long diskOffset, long byteCount)
        {
            var bytesPerBlock = _payload.StoreHeader.BytesPerBlock;
            var dataBlockEntryList = new List<DataBlockEntry>();
            if (diskOffset % bytesPerBlock != 0L)
                throw new ImageStorageException("Parameter 'diskOffset' must be a multiple of the block size.");
            var num1 = (uint) ((ulong) (byteCount + bytesPerBlock - 1L) / bytesPerBlock);
            var num2 = (uint) ((ulong) diskOffset / bytesPerBlock);
            for (uint index = 0; index < num1; ++index)
            {
                var dataBlockEntry = new DataBlockEntry(bytesPerBlock);
                dataBlockEntry.BlockLocationsOnDisk.Add(new DiskLocation(index + num2));
                var dataSource = dataBlockEntry.DataSource;
                dataSource.Source = DataBlockSource.DataSource.Disk;
                dataSource.StorageOffset = (num2 + index) * (ulong) bytesPerBlock;
                dataBlockEntryList.Add(dataBlockEntry);
            }

            return dataBlockEntryList;
        }

        private void ConvertEntriesToUseEndOfDisk(List<DataBlockEntry> entries, ulong totalSectorCount)
        {
            var num1 = _payload.StoreHeader.BytesPerBlock / _sectorSize;
            if (totalSectorCount / num1 > uint.MaxValue)
                throw new ImageStorageException(
                    "The image minimum sector count is too large to be addressed with a 32-bit block count.");
            var num2 = (uint) (totalSectorCount / num1);
            foreach (var entry in entries)
            {
                var blockIndex = entry.BlockLocationsOnDisk[0].BlockIndex;
                var num3 = (uint) ((int) num2 - (int) blockIndex - 1);
                entry.BlockLocationsOnDisk[0].AccessMethod = DiskLocation.DiskAccessMethod.DiskEnd;
                entry.BlockLocationsOnDisk[0].BlockIndex = num3;
            }
        }
    }
}