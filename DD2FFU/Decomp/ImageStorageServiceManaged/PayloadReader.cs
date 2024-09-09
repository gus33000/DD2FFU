// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.PayloadReader
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class PayloadReader
    {
        private readonly List<PayloadOffset> _payloadOffsets;
        private readonly FileStream _payloadStream;

        public PayloadReader(FileStream payloadStream)
        {
            _payloadStream = payloadStream;
            _payloadOffsets = [];
            int num = 1;
            for (int index = 1; index <= num; ++index)
            {
                StorePayload storePayload = new(false);
                storePayload.ReadMetadataFromStream(payloadStream);
                _payloadStream.Position += storePayload.StoreHeader.BytesPerBlock -
                                           (_payloadStream.Position % storePayload.StoreHeader.BytesPerBlock);
                _payloadOffsets.Add(new PayloadOffset
                {
                    Payload = storePayload
                });
                if (storePayload.StoreHeader.MajorVersion >= 2)
                {
                    num = storePayload.StoreHeader.NumberOfStores;
                }
            }

            long position = _payloadStream.Position;
            for (int index = 0; index < num; ++index)
            {
                PayloadOffset payloadOffset = _payloadOffsets[index];
                payloadOffset.Offset = position;
                ImageStoreHeader storeHeader = payloadOffset.Payload.StoreHeader;
                position += storeHeader.BytesPerBlock * storeHeader.StoreDataEntryCount;
            }
        }

        public ReadOnlyCollection<StorePayload> Payloads
        {
            get
            {
                List<StorePayload> storePayloadList = [];
                foreach (PayloadOffset payloadOffset in _payloadOffsets)
                {
                    storePayloadList.Add(payloadOffset.Payload);
                }

                return storePayloadList.AsReadOnly();
            }
        }

        public void WriteToDisk(SafeFileHandle diskHandle, StorePayload payload)
        {
            uint bytesPerBlock = payload.StoreHeader.BytesPerBlock;
            long num = (long)NativeImaging.GetSectorCount(nint.Zero, diskHandle) *
                      NativeImaging.GetSectorSize(nint.Zero, diskHandle);
            PayloadOffset payloadOffset = FindPayloadOffset(payload);
            if (payloadOffset == null)
            {
                throw new ImageStorageException("Unable to find store payload.");
            }

            _payloadStream.Position = payloadOffset.Offset;
            SafeFileHandle safeFileHandle = _payloadStream.SafeFileHandle;
            using VirtualMemoryPtr virtualMemoryPtr = new(bytesPerBlock);
            for (StorePayload.BlockPhase phase = StorePayload.BlockPhase.Phase1; phase != StorePayload.BlockPhase.Invalid; ++phase)
            {
                foreach (DataBlockEntry phaseEntry in payload.GetPhaseEntries(phase))
                {
                    Win32Exports.ReadFile(safeFileHandle, (nint)virtualMemoryPtr, bytesPerBlock, out uint bytesRead);
                    for (int index = 0; index < phaseEntry.BlockLocationsOnDisk.Count; ++index)
                    {
                        long distanceToMove =
                            phaseEntry.BlockLocationsOnDisk[index].BlockIndex * (long)bytesPerBlock;
                        if (phaseEntry.BlockLocationsOnDisk[index].AccessMethod ==
                            DiskLocation.DiskAccessMethod.DiskEnd)
                        {
                            distanceToMove = num - distanceToMove - bytesPerBlock;
                        }

                        Win32Exports.SetFilePointerEx(diskHandle, distanceToMove, out long newFileLocation,
                                                        Win32Exports.MoveMethod.FILE_BEGIN);
                        Win32Exports.WriteFile(diskHandle, (nint)virtualMemoryPtr, bytesPerBlock,
                            out uint bytesWritten);
                    }
                }
            }
        }

        public DataBlockStream GetDataBlockStream(StorePayload payload, int sectorSize, long totalByteCount)
        {
            PayloadOffset payloadOffset = FindPayloadOffset(payload);
            return payloadOffset == null
                ? throw new ImageStorageException("Unable to find store payload.")
                : new DataBlockStream(
                new ImagePayloadSource(_payloadStream, payload, payloadOffset.Offset, totalByteCount, false),
                payload.StoreHeader.BytesPerBlock);
        }

        public void ValidatePayloadPartitions(int sectorSize, long totalByteCount, StorePayload payload,
            uint partitionStyle, bool isMainOSStore, IULogger logger)
        {
            DataBlockStream dataBlockStream = GetDataBlockStream(payload, sectorSize, totalByteCount);
            if ((int)partitionStyle == (int)ImageConstants.PartitionTypeGpt)
            {
                GuidPartitionTable guidPartitionTable = new(sectorSize, logger);
                guidPartitionTable.ReadFromStream(dataBlockStream, true);
                if (isMainOSStore && guidPartitionTable.GetEntry(ImageConstants.MAINOS_PARTITION_NAME) == null)
                {
                    throw new ImageStorageException(string.Format(
                                        "{0}: The given FFU does not contain the partition '{1}'.", MethodBase.GetCurrentMethod().Name,
                                        ImageConstants.MAINOS_PARTITION_NAME));
                }
            }
            else
            {
                if ((int)partitionStyle != (int)ImageConstants.PartitionTypeMbr)
                {
                    throw new ImageStorageException("The payload contains an invalid partition style.");
                }

                MasterBootRecord masterBootRecord = new(logger, sectorSize);
                _ = masterBootRecord.ReadFromStream(dataBlockStream, MasterBootRecord.MbrParseType.Normal);
                if (masterBootRecord.FindPartitionByType(ImageConstants.MBR_METADATA_PARTITION_TYPE) == null)
                {
                    throw new ImageStorageException(string.Format(
                                        "{0}: The given FFU does not contain the partition '{1}'.", MethodBase.GetCurrentMethod().Name,
                                        ImageConstants.MBR_METADATA_PARTITION_NAME));
                }

                if (masterBootRecord.FindPartitionByName(ImageConstants.MAINOS_PARTITION_NAME) == null)
                {
                    throw new ImageStorageException(string.Format(
                                        "{0}: The given FFU does not contain the partition '{1}'.", MethodBase.GetCurrentMethod().Name,
                                        ImageConstants.MAINOS_PARTITION_NAME));
                }
            }
        }

        public void LogPayload(IULogger logger, bool logStoreHeader, bool logDataEntries)
        {
            foreach (PayloadOffset payloadOffset in _payloadOffsets)
            {
                payloadOffset.Payload.LogInfo(logger, logStoreHeader, logDataEntries);
            }
        }

        private PayloadOffset FindPayloadOffset(StorePayload payload)
        {
            for (int index = 0; index < _payloadOffsets.Count; ++index)
            {
                PayloadOffset payloadOffset = _payloadOffsets[index];
                if (payloadOffset.Payload == payload)
                {
                    return payloadOffset;
                }
            }

            return null;
        }

        private class PayloadOffset
        {
            public StorePayload Payload
            {
                get; set;
            }

            public long Offset
            {
                get; set;
            }
        }
    }
}