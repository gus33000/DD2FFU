// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.ImagePayloadSource
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class ImagePayloadSource : IBlockStreamSource
    {
        private readonly uint _blockSize;
        private readonly long _firstDataByte;
        private readonly StorePayload _payload;
        private readonly bool _recovery;
        private readonly Stream _stream;

        public ImagePayloadSource(Stream imageStream, StorePayload payload, long firstDataBlockIndex,
            long diskSizeInBytes, bool recovery)
        {
            _payload = payload;
            _blockSize = payload.StoreHeader.BytesPerBlock;
            _stream = imageStream;
            Length = diskSizeInBytes;
            _firstDataByte = firstDataBlockIndex;
            _recovery = recovery;
        }

        public void ReadBlock(uint blockIndex, byte[] buffer, int bufferIndex)
        {
            var diskOffset = blockIndex * (long) _blockSize;
            if (diskOffset > Length)
                throw new ImageStorageException("Attempting to read beyond the end of the disk.");
            var offsetForDiskOffset = GetDataBlockOffsetForDiskOffset(diskOffset);
            if (offsetForDiskOffset == -1L)
            {
                Array.Clear(buffer, bufferIndex, (int) _blockSize);
            }
            else
            {
                _stream.Position = _firstDataByte + offsetForDiskOffset;
                _stream.Read(buffer, bufferIndex, (int) _blockSize);
            }
        }

        public long Length { get; }

        private long GetLocationDiskOffset(DiskLocation location)
        {
            if (location.AccessMethod == DiskLocation.DiskAccessMethod.DiskBegin)
                return location.BlockIndex * (long) _blockSize;
            return Length - (location.BlockIndex + 1U) * _blockSize;
        }

        private long GetDataBlockOffsetForDiskOffset(long diskOffset)
        {
            long num1 = -1;
            var num2 = _payload.StoreHeader.StoreDataEntryCount - 1U;
            var phase = _recovery ? StorePayload.BlockPhase.Phase3 : StorePayload.BlockPhase.Invalid;
            do
            {
                --phase;
                var phaseEntries = _payload.GetPhaseEntries(phase);
                for (var index1 = 0; index1 < phaseEntries.Count; ++index1)
                {
                    var dataBlockEntryList = phaseEntries;
                    var dataBlockEntry = dataBlockEntryList[dataBlockEntryList.Count - (index1 + 1)];
                    for (var index2 = 0; index2 < dataBlockEntry.BlockLocationsOnDisk.Count; ++index2)
                        if (diskOffset == GetLocationDiskOffset(dataBlockEntry.BlockLocationsOnDisk[index2]))
                        {
                            num1 = _blockSize * (long) num2;
                            break;
                        }

                    if (num1 <= 0L)
                        --num2;
                    else
                        break;
                }
            } while (num1 < 0L && phase != StorePayload.BlockPhase.Phase1);

            return num1;
        }
    }
}