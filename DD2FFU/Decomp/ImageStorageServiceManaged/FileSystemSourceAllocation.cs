// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.FileSystemSourceAllocation
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class FileSystemSourceAllocation : ISourceAllocation
    {
        internal static Guid OriginalSystemPartition = Guid.Empty;
        private readonly byte[] _blockAllocationBitmap;
        private readonly uint _blockCount;
        private readonly uint _blockSize;
        private readonly string _partitionName;
        private readonly ulong _partitionOffset;
        private readonly ImageStorage _storage;

        public FileSystemSourceAllocation(ImageStorage imageService, string partitionName, ulong partitionOffset,
            uint blockSize)
        {
            _storage = imageService;
            _blockSize = blockSize;
            _partitionOffset = partitionOffset;
            _partitionName = partitionName;
            ulong partitionSize =
                NativeImaging.GetPartitionSize((nint)_storage.ServiceHandle, _storage.StoreId, partitionName);
            uint sectorSize = NativeImaging.GetSectorSize((nint)_storage.ServiceHandle, _storage.StoreId);
            ulong num = partitionSize * sectorSize;
            if ((long)(num / sectorSize) != (long)partitionSize)
            {
                throw new ImageStorageException(
                                string.Format("Volume {0} is too large to be byte-addressed with a 64-bit value.", partitionName));
            }

            _blockCount = (uint)((num + blockSize - 1UL) / blockSize);
            if (_blockCount * (ulong)blockSize < num)
            {
                throw new ImageStorageException(
                                string.Format("Volume {0} is too large to access with a 32-bit block count.", partitionName));
            }

            _blockAllocationBitmap = new byte[(int)((_blockCount + 7U) / 8U)];
            NativeImaging.GetBlockAllocationBitmap((nint)_storage.ServiceHandle, _storage.StoreId, partitionName,
                _blockSize, _blockAllocationBitmap);
        }

        public bool this[uint blockIndex]
        {
            get
            {
                byte num = (byte)(1 << ((int)blockIndex & 7));
                return (_blockAllocationBitmap[(int)(blockIndex / 8U)] & (uint)num) > 0U;
            }
        }

        public uint GetAllocationSize()
        {
            return _blockSize;
        }

        public bool BlockIsAllocated(ulong diskByteOffset)
        {
            return this[(uint)((diskByteOffset - _partitionOffset) / _blockSize)];
        }

        public List<DataBlockEntry> GenerateDataEntries()
        {
            List<DataBlockEntry> dataBlockEntryList = [];
            uint num = (uint)(_partitionOffset / _blockSize);
            for (uint index = 0; index < _blockCount; ++index)
            {
                if (this[index])
                {
                    DataBlockEntry dataBlockEntry = new(_blockSize);
                    dataBlockEntry.BlockLocationsOnDisk.Add(new DiskLocation(index + num));
                    DataBlockSource dataSource = dataBlockEntry.DataSource;
                    dataSource.Source = DataBlockSource.DataSource.Disk;
                    dataSource.StorageOffset = (num + index) * (ulong)_blockSize;
                    dataBlockEntryList.Add(dataBlockEntry);
                }
            }

            return dataBlockEntryList;
        }

        [Conditional("DEBUG")]
        public void ValidateDataEntries(List<DataBlockEntry> entries)
        {
            byte[] numArray = new byte[(int)((_blockCount + 7U) / 8U)];
            Array.Clear(numArray, 0, ((int)_blockCount & 7) / 8);
            uint num1 = (uint)(_partitionOffset / _blockSize);
            foreach (DataBlockEntry entry in entries)
            {
                uint num2 = entry.BlockLocationsOnDisk[0].BlockIndex - num1;
                numArray[(int)(num2 / 8U)] = (byte)(numArray[(int)(num2 / 8U)] | (1U << (int)(num2 % 8U)));
            }

            for (int index = 0; index < numArray.Length; ++index)
            {
                if (numArray[index] != _blockAllocationBitmap[index])
                {
                    throw new ImageStorageException(string.Format(
                                        "The block bitmap generated from the volume doesn't match the bitmap generated from the data entries at offset {0}",
                                        index));
                }
            }
        }
    }
}