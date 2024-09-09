// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.VirtualDiskPayloadGenerator
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class VirtualDiskPayloadGenerator : IDisposable
    {
        private bool _alreadyDisposed;
        private readonly uint _blockSize;
        private readonly IULogger _logger;
        private readonly ushort _numOfStores;
        private StorePayload _payload;
        private readonly bool _recovery;
        private SafeFileHandle _sourceHandle;
        private readonly ImageStorage _storage;
        private readonly ushort _storeHeaderVersion;
        private readonly ushort _storeIndex;
        private VirtualDiskSourceAllocation _virtualDiskAllocator;

        public VirtualDiskPayloadGenerator(IULogger logger, uint bytesPerBlock, ImageStorage storage,
            ushort storeHeaderVersion, ushort numOfStores, ushort storeIndex, bool recovery)
        {
            _sourceHandle = storage.SafeStoreHandle;
            _blockSize = bytesPerBlock;
            _storage = storage;
            _virtualDiskAllocator =
                (int)storage.VirtualHardDiskSectorSize != (int)ImageConstants.DefaultVirtualHardDiskSectorSize
                    ? null
                    : new VirtualDiskSourceAllocation(storage.VirtualDiskFilePath, bytesPerBlock);
            _logger = logger;
            _storeHeaderVersion = storeHeaderVersion;
            _numOfStores = numOfStores;
            _storeIndex = storeIndex;
            _recovery = recovery;
            _payload = new StorePayload(recovery);
        }

        internal long TotalSize
        {
            get
            {
                long metadataSize = _payload.GetMetadataSize();
                return metadataSize + GetPaddingSizeInBytes(metadataSize) + GetBlockDataSize();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void GenerateStorePayload(ImageStorage storage)
        {
            if (_storeHeaderVersion == 2)
            {
                _payload.StoreHeader.Initialize2(FullFlashUpdateType.FullUpdate, _blockSize, storage.Image,
                                _numOfStores, _storeIndex, storage.Store.DevicePath);
            }
            else
            {
                _payload.StoreHeader.Initialize(FullFlashUpdateType.FullUpdate, _blockSize, storage.Image);
            }

            SafeFileHandle sourceHandle = _sourceHandle;
            ImageStorage storage1 = storage;
            int hardDiskSectorSize = (int)storage1.VirtualHardDiskSectorSize;
            VirtualDiskSourceAllocation virtualDiskAllocator = _virtualDiskAllocator;
            GenerateDataEntries(sourceHandle, storage1, (uint)hardDiskSectorSize, virtualDiskAllocator);
            _payload.StoreHeader.StoreDataEntryCount =
                (uint)(_payload.Phase1DataEntries.Count + _payload.Phase2DataEntries.Count);
            if (!_recovery)
            {
                _payload.StoreHeader.StoreDataEntryCount += (uint)_payload.Phase3DataEntries.Count;
            }

            if (_storeHeaderVersion != 2)
            {
                return;
            }

            _payload.StoreHeader.StorePayloadSize = (ulong)GetBlockDataSize();
        }

        public void WriteMetadata(IPayloadWrapper payloadWrapper)
        {
            byte[] metadata = _payload.GetMetadata(_blockSize);
            payloadWrapper.Write(metadata);
        }

        public void WriteStorePayload(IPayloadWrapper payloadWrapper)
        {
            _ = new byte[(int)_blockSize];
            uint bytesRead = 0;
            using VirtualMemoryPtr virtualMemoryPtr = new(_payload.StoreHeader.BytesPerBlock);
            StorePayload.BlockPhase phase = StorePayload.BlockPhase.Phase1;
            for (StorePayload.BlockPhase blockPhase = _recovery ? StorePayload.BlockPhase.Phase3 : StorePayload.BlockPhase.Invalid;
                phase != blockPhase;
                ++phase)
            {
                foreach (DataBlockEntry phaseEntry in _payload.GetPhaseEntries(phase))
                {
                    byte[] numArray2 = new byte[(int)_blockSize];
                    if (phaseEntry.DataSource.Source == DataBlockSource.DataSource.Disk)
                    {
                        Win32Exports.SetFilePointerEx(_sourceHandle, (long)phaseEntry.DataSource.StorageOffset,
                            out long newFileLocation, Win32Exports.MoveMethod.FILE_BEGIN);
                        Win32Exports.ReadFile(_sourceHandle, virtualMemoryPtr.AllocatedPointer,
                            _payload.StoreHeader.BytesPerBlock, out bytesRead);
                        Marshal.Copy(virtualMemoryPtr.AllocatedPointer, numArray2, 0,
                            (int)_payload.StoreHeader.BytesPerBlock);
                    }
                    else if (phaseEntry.DataSource.Source == DataBlockSource.DataSource.Memory)
                    {
                        numArray2 = phaseEntry.DataSource.GetMemoryData();
                    }

                    ReplaceGptDiskId(numArray2, FileSystemSourceAllocation.OriginalSystemPartition,
                        ImageConstants.SYSTEM_PARTITION_ID);
                    payloadWrapper.Write(numArray2);
                }
            }
        }

        public void Finalize(IPayloadWrapper payloadWrapper)
        {
            payloadWrapper.FinalizeWrapper();
        }

        public uint GetPaddingSizeInBytes(long currentSize)
        {
            return _blockSize - (uint)((ulong)currentSize % _blockSize);
        }

        private void ReplaceGptDiskId(byte[] data, Guid originalPartitionId, Guid newPartitionId)
        {
            if (!(originalPartitionId != Guid.Empty) || !(newPartitionId != Guid.Empty))
            {
                return;
            }

            byte[] byteArray1 = originalPartitionId.ToByteArray();
            byte[] byteArray2 = newPartitionId.ToByteArray();
            for (int index1 = 0; index1 < data.Length - byteArray1.Length; ++index1)
            {
                bool flag = true;
                for (int index2 = 0; index2 < byteArray1.Length; ++index2)
                {
                    if (data[index1 + index2] != byteArray1[index2])
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    for (int index2 = 0; index2 < byteArray2.Length; ++index2)
                    {
                        data[index1 + index2] = byteArray2[index2];
                    }

                    index1 += byteArray2.Length - 1;
                }
            }
        }

        private void GenerateDataEntries(SafeFileHandle sourceHandle, ImageStorage storage, uint sourceSectorSize,
            ISourceAllocation sourceAllocation)
        {
            using DataBlockStream dataBlockStream =
                new(new DiskStreamSource(sourceHandle, _blockSize), _blockSize);
            MasterBootRecord masterBootRecord = new(_logger, (int)sourceSectorSize);
            _ = masterBootRecord.ReadFromStream(dataBlockStream, MasterBootRecord.MbrParseType.Normal);
            (!masterBootRecord.IsValidProtectiveMbr()
                ? new MbrDataEntryGenerator(_logger, _payload, sourceAllocation, sourceHandle, sourceSectorSize,
                    storage.Image, _recovery)
                : (IEntryGenerator)new GptDataEntryGenerator(storage, _payload, sourceAllocation, sourceHandle,
                    sourceSectorSize, _recovery)).GenerateEntries(storage.Store.OnlyAllocateDefinedGptEntries);
        }

        ~VirtualDiskPayloadGenerator()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadyDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                _payload = null;
                _sourceHandle = null;
                if (_virtualDiskAllocator != null)
                {
                    _virtualDiskAllocator.Dispose();
                    _virtualDiskAllocator = null;
                }
            }

            _alreadyDisposed = true;
        }

        private long GetBlockDataSize()
        {
            long num = 0;
            StorePayload.BlockPhase phase = StorePayload.BlockPhase.Phase1;
            for (StorePayload.BlockPhase blockPhase = _recovery ? StorePayload.BlockPhase.Phase3 : StorePayload.BlockPhase.Invalid;
                phase != blockPhase;
                ++phase)
            {
                System.Collections.Generic.List<DataBlockEntry> phaseEntries = _payload.GetPhaseEntries(phase);
                num += _payload.StoreHeader.BytesPerBlock * phaseEntries.Count;
            }

            return num;
        }
    }
}