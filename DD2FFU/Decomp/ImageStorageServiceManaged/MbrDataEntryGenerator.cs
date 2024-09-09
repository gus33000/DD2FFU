// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.MbrDataEntryGenerator
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class MbrDataEntryGenerator : IEntryGenerator
    {
        private readonly ISourceAllocation _allocation;
        private readonly List<FullFlashUpdateImage.FullFlashUpdatePartition> _finalPartitions;
        private readonly FullFlashUpdateImage _image;
        private readonly IULogger _logger;
        private readonly StorePayload _payload;
        private List<DataBlockEntry> _phase2PartitionTableEntries;
        private readonly bool _recovery;
        private readonly uint _sectorSize;
        private readonly SafeFileHandle _sourceHandle;
        private MasterBootRecord _table;

        public MbrDataEntryGenerator(IULogger logger, StorePayload storePayload, ISourceAllocation sourceAllocation,
            SafeFileHandle sourceHandle, uint sourceSectorSize, FullFlashUpdateImage image, bool recovery)
        {
            _payload = storePayload;
            _allocation = sourceAllocation;
            _sourceHandle = sourceHandle;
            _image = image;
            _sectorSize = sourceSectorSize;
            _table = new MasterBootRecord(logger, (int)sourceSectorSize);
            _finalPartitions = [];
            _logger = logger;
            _recovery = recovery;
            if (storePayload.StoreHeader.BytesPerBlock < image.Stores[0].SectorSize)
            {
                throw new ImageStorageException("The data block size is less than the device sector size.");
            }

            if (storePayload.StoreHeader.BytesPerBlock % image.Stores[0].SectorSize != 0U)
            {
                throw new ImageStorageException("The data block size is not a multiple of the device sector size.");
            }

            if (storePayload.StoreHeader.BytesPerBlock > sourceAllocation.GetAllocationSize())
            {
                throw new ImageStorageException(
                                "The payload block size is larger than the allocation size of the temp store.");
            }

            if (sourceAllocation.GetAllocationSize() % storePayload.StoreHeader.BytesPerBlock != 0U)
            {
                throw new ImageStorageException(
                                "The allocation size of the temp store is not a multiple of the payload block size.");
            }
        }

        public void GenerateEntries(bool onlyAllocateDefinedGptEntries)
        {
            _payload.Phase1DataEntries = GeneratePhase1Entries();
            _payload.Phase2DataEntries = GeneratePhase2Entries();
            if (_recovery)
            {
                return;
            }

            _payload.Phase3DataEntries = GeneratePhase3Entries();
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
                        new DiskLocation((uint) index, DiskLocation.DiskAccessMethod.DiskBegin)
                    }
                });
            }

            _payload.StoreHeader.InitialPartitionTableBlockCount = (uint)dataBlockEntryList.Count;
            return dataBlockEntryList;
        }

        private List<DataBlockEntry> GeneratePhase2Entries()
        {
            DiskStreamSource diskStreamSource = new(_sourceHandle, _payload.StoreHeader.BytesPerBlock);
            List<DataBlockEntry> dataBlocks = [];
            List<string> stringList = [];
            int bytesPerBlock = (int)_payload.StoreHeader.BytesPerBlock;
            using (DataBlockStream dataBlockStream = new(diskStreamSource, (uint)bytesPerBlock))
            {
                _ = _table.ReadFromStream(dataBlockStream, MasterBootRecord.MbrParseType.Normal);
                for (int index = 0; index < _image.Stores[0].PartitionCount; ++index)
                {
                    FullFlashUpdateImage.FullFlashUpdatePartition partition = _image.Stores[0].Partitions[index];
                    if (partition.RequiredToFlash)
                    {
                        stringList.Add(partition.Name);
                    }
                    else
                    {
                        _finalPartitions.Add(partition);
                        _table.RemovePartition(partition.Name);
                    }
                }

                _table.DiskSignature = ImageConstants.SYSTEM_STORE_SIGNATURE;
                _table.WriteToStream(dataBlockStream, false);
                _phase2PartitionTableEntries = dataBlockStream.BlockEntries;
            }

            foreach (string partitionName in stringList)
            {
                MbrPartitionEntry partitionByName = _table.FindPartitionByName(partitionName);
                if (partitionByName.AbsoluteStartingSector > 0U && partitionByName.StartingSector > 0U)
                {
                    ulong num = partitionByName.SectorCount * (ulong)_sectorSize;
                    List<DataBlockEntry> dataEntriesFromDisk =
                        GenerateDataEntriesFromDisk(partitionByName.AbsoluteStartingSector * (long)_sectorSize,
                            (long)num);
                    int count = dataEntriesFromDisk.Count;
                    FilterUnAllocatedDataEntries(dataEntriesFromDisk);
                    _logger.LogInfo("Recording (Phase2) {0} of {1} blocks from partition {2} ({3} bytes)",
                        dataEntriesFromDisk.Count, count,
                        _table.GetPartitionName(partitionByName),
                        dataEntriesFromDisk.Count * _payload.StoreHeader.BytesPerBlock);
                    dataBlocks.AddRange(dataEntriesFromDisk);
                }
            }

            FilterPartitionTablesFromDataBlocks(dataBlocks);
            _payload.StoreHeader.FlashOnlyPartitionTableBlockCount = (uint)_phase2PartitionTableEntries.Count;
            _payload.StoreHeader.FlashOnlyPartitionTableBlockIndex =
                _payload.StoreHeader.InitialPartitionTableBlockCount + (uint)dataBlocks.Count;
            dataBlocks.AddRange(_phase2PartitionTableEntries);
            return dataBlocks;
        }

        private List<DataBlockEntry> GeneratePhase3Entries()
        {
            List<DataBlockEntry> dataBlocks = [];
            using (DataBlockStream dataBlockStream =
                new(new DiskStreamSource(_sourceHandle, _payload.StoreHeader.BytesPerBlock),
                    _payload.StoreHeader.BytesPerBlock))
            {
                _table = new MasterBootRecord(_logger, (int)_sectorSize);
                _ = _table.ReadFromStream(dataBlockStream, MasterBootRecord.MbrParseType.Normal);
                _table.DiskSignature = ImageConstants.SYSTEM_STORE_SIGNATURE;
                _table.WriteToStream(dataBlockStream, false);
                foreach (FullFlashUpdateImage.FullFlashUpdatePartition finalPartition in _finalPartitions)
                {
                    MbrPartitionEntry partitionByName = _table.FindPartitionByName(finalPartition.Name);
                    if (partitionByName.AbsoluteStartingSector > 0U && partitionByName.StartingSector > 0U)
                    {
                        ulong num = partitionByName.SectorCount * (ulong)_sectorSize;
                        List<DataBlockEntry> dataEntriesFromDisk =
                            GenerateDataEntriesFromDisk(partitionByName.AbsoluteStartingSector * (long)_sectorSize,
                                (long)num);
                        int count = dataEntriesFromDisk.Count;
                        FilterUnAllocatedDataEntries(dataEntriesFromDisk);
                        _logger.LogInfo("Recording (Phase3) {0} of {1} blocks from partition {2} ({3} bytes)",
                            dataEntriesFromDisk.Count, count, finalPartition.Name,
                            dataEntriesFromDisk.Count * _payload.StoreHeader.BytesPerBlock);
                        dataBlocks.AddRange(dataEntriesFromDisk);
                    }
                }

                FilterPartitionTablesFromDataBlocks(dataBlocks);
                if (!_recovery)
                {
                    _payload.StoreHeader.FinalPartitionTableBlockCount = (uint)dataBlockStream.BlockEntries.Count;
                    _payload.StoreHeader.FinalPartitionTableBlockIndex =
                        (uint)((int)_payload.StoreHeader.FlashOnlyPartitionTableBlockIndex +
                                (int)_payload.StoreHeader.FlashOnlyPartitionTableBlockCount + dataBlocks.Count);
                }
                else
                {
                    _payload.StoreHeader.FinalPartitionTableBlockCount = 0U;
                    _payload.StoreHeader.FinalPartitionTableBlockIndex =
                        _payload.StoreHeader.FlashOnlyPartitionTableBlockIndex +
                        _payload.StoreHeader.FlashOnlyPartitionTableBlockCount;
                }

                dataBlocks.AddRange(dataBlockStream.BlockEntries);
            }

            return dataBlocks;
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
            _ = (int)_payload.StoreHeader.BytesPerBlock;
            for (int index = 0; index < dataBlocks.Count; ++index)
            {
                DataBlockEntry dataBlock = dataBlocks[index];
                if (dataBlock.DataSource.Source == DataBlockSource.DataSource.Disk &&
                    !_allocation.BlockIsAllocated(dataBlock.DataSource.StorageOffset))
                {
                    dataBlocks.RemoveAt(index--);
                }
            }
        }

        private List<DataBlockEntry> GenerateDataEntriesFromDisk(long diskOffset, long byteCount)
        {
            uint bytesPerBlock = _payload.StoreHeader.BytesPerBlock;
            List<DataBlockEntry> dataBlockEntryList = [];
            uint num1 = (uint)((ulong)byteCount / bytesPerBlock);
            if (byteCount % bytesPerBlock != 0L)
            {
                ++num1;
            }

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
    }
}