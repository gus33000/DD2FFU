// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.GptDataEntryGenerator
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DD2FFU;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class GptDataEntryGenerator : IEntryGenerator
    {
        private readonly List<FullFlashUpdateImage.FullFlashUpdatePartition> _finalPartitions;
        private readonly FullFlashUpdateImage _image;
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
            _table = new GuidPartitionTable((int)sourceSectorSize, storage.Logger);
            _finalPartitions = [];
            _storage = storage;
            _recovery = recovery;
            if (storePayload.StoreHeader.BytesPerBlock < _storage.Store.SectorSize)
            {
                throw new ImageStorageException("The data block size is less than the device sector size.");
            }

            if (storePayload.StoreHeader.BytesPerBlock % _storage.Store.SectorSize != 0U)
            {
                throw new ImageStorageException("The data block size is not a multiple of the device sector size.");
            }

            if (storageAllocation == null)
            {
                return;
            }

            if (storePayload.StoreHeader.BytesPerBlock > storageAllocation.GetAllocationSize())
            {
                throw new ImageStorageException(
                                "The payload block size is larger than the allocation size of the temp store.");
            }

            if (storageAllocation.GetAllocationSize() % storePayload.StoreHeader.BytesPerBlock != 0U)
            {
                throw new ImageStorageException(
                                "The allocation size of the temp store is not a multiple of the payload block size.");
            }
        }

        public void GenerateEntries(bool onlyAllocateDefinedGptEntries)
        {
            _payload.Phase1DataEntries = GeneratePhase1Entries();
            _payload.Phase2DataEntries = GeneratePhase2Entries(onlyAllocateDefinedGptEntries);
            if (_recovery)
            {
                return;
            }

            _payload.Phase3DataEntries = GeneratePhase3Entries(onlyAllocateDefinedGptEntries);
        }

        private List<DataBlockEntry> GeneratePhase1Entries()
        {
            List<DataBlockEntry> dataBlockEntryList = new((int)(131072U / _payload.StoreHeader.BytesPerBlock));
            for (int index = 0; index < dataBlockEntryList.Capacity; ++index)
            {
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
            }

            _payload.StoreHeader.InitialPartitionTableBlockCount = (uint)dataBlockEntryList.Count;
            return dataBlockEntryList;
        }

        private List<DataBlockEntry> GeneratePhase2Entries(bool onlyAllocateDefinedGptEntries)
        {
            DataBlockStream dataBlockStream =
                new(new DiskStreamSource(_sourceHandle, _payload.StoreHeader.BytesPerBlock),
                    _payload.StoreHeader.BytesPerBlock);
            _table.ReadFromStream(dataBlockStream, true);
            _table.ValidateCrcs();
            for (int index = 0; index < _storage.Store.PartitionCount; ++index)
            {
                FullFlashUpdateImage.FullFlashUpdatePartition partition = _storage.Store.Partitions[index];
                if (!partition.RequiredToFlash)
                {
                    _finalPartitions.Add(partition);
                    _table.RemoveEntry(partition.Name);
                }
            }

            if (_storage.IsMainOSStorage)
            {
                _table.NormalizeGptIds(out FileSystemSourceAllocation.OriginalSystemPartition);
            }

            _table.FixCrcs();
            _table.WriteToStream(dataBlockStream, true, onlyAllocateDefinedGptEntries);
            _phase2PartitionTableEntries = dataBlockStream.BlockEntries;
            List<DataBlockEntry> dataBlockEntryList = [];
            foreach (GuidPartitionTableEntry entry in _table.Entries)
            {
                if (entry.StartingSector > 0UL)
                {
                    GenerateDataEntries(dataBlockEntryList, entry);
                }
            }

            FilterPartitionTablesFromDataBlocks(dataBlockEntryList);
            _payload.StoreHeader.FlashOnlyPartitionTableBlockCount = (uint)_phase2PartitionTableEntries.Count;
            _payload.StoreHeader.FlashOnlyPartitionTableBlockIndex =
                _payload.StoreHeader.InitialPartitionTableBlockCount + (uint)dataBlockEntryList.Count;
            dataBlockEntryList.AddRange(_phase2PartitionTableEntries);
            return dataBlockEntryList;
        }

        private List<DataBlockEntry> GeneratePhase3Entries(bool onlyAllocateDefinedGptEntries)
        {
            List<DataBlockEntry> dataBlockEntryList = [];
            DiskStreamSource diskStreamSource = new(_sourceHandle, _payload.StoreHeader.BytesPerBlock);
            DataBlockStream dataBlockStream1 = new(diskStreamSource, _payload.StoreHeader.BytesPerBlock);
            _table.ReadFromStream(dataBlockStream1, true);
            _table.ValidateCrcs();
            while (_finalPartitions.Count > 0)
            {
                foreach (GuidPartitionTableEntry entry in _table.Entries)
                {
                    if (string.Compare(entry.PartitionName, _finalPartitions[0].Name, true,
                                            CultureInfo.InvariantCulture) == 0)
                    {
                        GenerateDataEntries(dataBlockEntryList, entry);
                        _finalPartitions.RemoveAt(0);
                        break;
                    }
                }
            }

            GuidPartitionTableEntry entry1 = _table.GetEntry(ImageConstants.MAINOS_PARTITION_NAME);
            if (entry1 != null)
            {
                entry1.Attributes &= ~ImageConstants.GPT_ATTRIBUTE_NO_DRIVE_LETTER;
            }

            if (_storage.IsMainOSStorage)
            {
                _table.NormalizeGptIds(out FileSystemSourceAllocation.OriginalSystemPartition);
            }

            _table.FixCrcs();
            _table.WriteToStream(dataBlockStream1, true, onlyAllocateDefinedGptEntries);
            FilterPartitionTablesFromDataBlocks(dataBlockEntryList);
            if (!_recovery)
            {
                _payload.StoreHeader.FinalPartitionTableBlockCount = (uint)dataBlockStream1.BlockEntries.Count;
                _payload.StoreHeader.FinalPartitionTableBlockIndex =
                    (uint)((int)_payload.StoreHeader.FlashOnlyPartitionTableBlockIndex +
                            (int)_payload.StoreHeader.FlashOnlyPartitionTableBlockCount + dataBlockEntryList.Count);
            }
            else
            {
                _payload.StoreHeader.FinalPartitionTableBlockCount = 0U;
                _payload.StoreHeader.FinalPartitionTableBlockIndex =
                    _payload.StoreHeader.FlashOnlyPartitionTableBlockIndex +
                    _payload.StoreHeader.FlashOnlyPartitionTableBlockCount;
            }

            dataBlockEntryList.AddRange(dataBlockStream1.BlockEntries);
            DataBlockStream dataBlockStream2 = new(diskStreamSource, _payload.StoreHeader.BytesPerBlock);
            _table.ReadFromStream(dataBlockStream2, false);
            List<DataBlockEntry> blockEntries = dataBlockStream2.BlockEntries;
            _payload.StoreHeader.FinalPartitionTableBlockCount += (uint)dataBlockStream2.BlockEntries.Count;
            ConvertEntriesToUseEndOfDisk(blockEntries, NativeImaging.GetSectorCount(nint.Zero, _sourceHandle));
            dataBlockEntryList.AddRange(blockEntries);
            return dataBlockEntryList;
        }

        private void GenerateDataEntries(List<DataBlockEntry> dataEntries, GuidPartitionTableEntry entry)
        {
            List<DataBlockEntry> dataBlocks;
            if (!_storage.PartitionIsMountedRaw(entry.PartitionName))
            {
                ulong partitionOffset =
                    NativeImaging.GetPartitionOffset((nint)_storage.ServiceHandle, _storage.StoreId,
                        entry.PartitionName) * _sectorSize;
                dataBlocks = new FileSystemSourceAllocation(_storage, entry.PartitionName, partitionOffset,
                    _payload.StoreHeader.BytesPerBlock).GenerateDataEntries();
            }
            else if (!_storage.IsBackingFileVhdx() || _storage.IsPartitionTargeted(entry.PartitionName))
            {
                ulong num = (entry.LastSector + 1UL - entry.StartingSector) * _sectorSize;
                dataBlocks = GenerateDataEntriesFromDisk((long)entry.StartingSector * _sectorSize, (long)num);
                FilterUnAllocatedDataEntries(dataBlocks);
            }
            else
            {
                dataBlocks = [];
            }

            dataEntries.AddRange(dataBlocks);
        }

        private void FilterPartitionTablesFromDataBlocks(List<DataBlockEntry> dataBlocks)
        {
            int bytesPerBlock = (int)_payload.StoreHeader.BytesPerBlock;
            foreach (DataBlockEntry dataBlock in dataBlocks)
            {
                foreach (DataBlockEntry partitionTableEntry in _phase2PartitionTableEntries)
                {
                    if ((int)dataBlock.BlockLocationsOnDisk[0].BlockIndex ==
                                        (int)partitionTableEntry.BlockLocationsOnDisk[0].BlockIndex)
                    {
                        dataBlock.DataSource = new DataBlockSource();
                        dataBlock.DataSource.Source = partitionTableEntry.DataSource.Source;
                        dataBlock.DataSource.SetMemoryData(partitionTableEntry.DataSource.GetMemoryData(), 0,
                            bytesPerBlock);
                        break;
                    }
                }
            }
        }

        private void FilterUnAllocatedDataEntries(List<DataBlockEntry> dataBlocks)
        {
            int bytesPerBlock = (int)_payload.StoreHeader.BytesPerBlock;
            for (int index = 0; index < dataBlocks.Count; ++index)
            {
                DataBlockEntry dataBlock = dataBlocks[index];

                List<GuidPartitionTableEntry> parts = _table.Entries;

                if ((dataBlock.DataSource.Source == DataBlockSource.DataSource.Disk && _storageAllocation != null &&
                    !_storageAllocation.BlockIsAllocated(dataBlock.DataSource.StorageOffset)) || parts.Any(x => Constants.partitions.Any(y => y.ToLower() == x.PartitionName.ToLower() && x.StartingSector * 0x200 <= dataBlock.DataSource.StorageOffset && dataBlock.DataSource.StorageOffset <= x.LastSector * 0x200)))
                {
                    dataBlocks.RemoveAt(index--);
                }
            }
        }

        private List<DataBlockEntry> GenerateDataEntriesFromDisk(long diskOffset, long byteCount)
        {
            uint bytesPerBlock = _payload.StoreHeader.BytesPerBlock;
            List<DataBlockEntry> dataBlockEntryList = [];
            if (diskOffset % bytesPerBlock != 0L)
            {
                throw new ImageStorageException("Parameter 'diskOffset' must be a multiple of the block size.");
            }

            uint num1 = (uint)((ulong)(byteCount + bytesPerBlock - 1L) / bytesPerBlock);
            uint num2 = (uint)((ulong)diskOffset / bytesPerBlock);
            for (uint index = 0; index < num1; ++index)
            {
                DataBlockEntry dataBlockEntry = new(bytesPerBlock);
                dataBlockEntry.BlockLocationsOnDisk.Add(new DiskLocation(index + num2));
                DataBlockSource dataSource = dataBlockEntry.DataSource;
                dataSource.Source = DataBlockSource.DataSource.Disk;
                dataSource.StorageOffset = (num2 + index) * (ulong)bytesPerBlock;
                dataBlockEntryList.Add(dataBlockEntry);
            }

            return dataBlockEntryList;
        }

        private void ConvertEntriesToUseEndOfDisk(List<DataBlockEntry> entries, ulong totalSectorCount)
        {
            uint num1 = _payload.StoreHeader.BytesPerBlock / _sectorSize;
            if (totalSectorCount / num1 > uint.MaxValue)
            {
                throw new ImageStorageException(
                                "The image minimum sector count is too large to be addressed with a 32-bit block count.");
            }

            uint num2 = (uint)(totalSectorCount / num1);
            foreach (DataBlockEntry entry in entries)
            {
                uint blockIndex = entry.BlockLocationsOnDisk[0].BlockIndex;
                uint num3 = (uint)((int)num2 - (int)blockIndex - 1);
                entry.BlockLocationsOnDisk[0].AccessMethod = DiskLocation.DiskAccessMethod.DiskEnd;
                entry.BlockLocationsOnDisk[0].BlockIndex = num3;
            }
        }
    }
}