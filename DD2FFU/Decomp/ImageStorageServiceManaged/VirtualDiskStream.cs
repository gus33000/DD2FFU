// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.VirtualDiskStream
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class VirtualDiskStream : Stream
    {
        private long _position;
        private readonly byte[] _sectorBuffer;
        private uint _sectorBufferIndex = uint.MaxValue;

        public VirtualDiskStream(DynamicHardDisk virtualDisk)
        {
            VirtualDisk = virtualDisk;
            _sectorBuffer = new byte[(int)virtualDisk.SectorSize];
        }

        private DynamicHardDisk VirtualDisk
        {
            get;
        }

        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override bool CanSeek => true;

        public override bool CanTimeout => false;

        public override long Length => (long)VirtualDisk.SectorCount * VirtualDisk.SectorSize;

        public override long Position
        {
            get => _position;
            set
            {
                if (value > Length)
                {
                    throw new ImageStorageException("The given position is beyond the end of the image payload.");
                }

                _position = value;
            }
        }

        public override void Flush()
        {
            VirtualDisk.FlushFile();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (offset > Length)
            {
                throw new ImageStorageException("The  offset is beyond the end of the image.");
            }

            switch (origin)
            {
                case SeekOrigin.Begin:
                    _position = offset;
                    return _position;
                case SeekOrigin.Current:
                    if (offset == 0L)
                    {
                        return _position;
                    }

                    if (offset < 0L)
                    {
                        throw new ImageStorageException("Negative offsets are not implemented.");
                    }

                    if (_position >= Length)
                    {
                        throw new ImageStorageException("The offset is beyond the end of the image.");
                    }

                    if (Length - _position < offset)
                    {
                        throw new ImageStorageException("The offset is beyond the end of the image.");
                    }

                    _position = offset;
                    return _position;
                case SeekOrigin.End:
                    if (offset > 0L)
                    {
                        throw new ImageStorageException("The offset is beyond the end of the image.");
                    }

                    if (Length + offset < 0L)
                    {
                        throw new ImageStorageException("The offset is invalid.");
                    }

                    _position = Length + offset;
                    return _position;
                default:
                    throw new ImageStorageException("The origin parameter is invalid.");
            }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int num1 = 0;
            while (count > 0)
            {
                uint num2 = (uint)((ulong)Position / VirtualDisk.SectorSize);
                uint num3 = (uint)((ulong)Position % VirtualDisk.SectorSize);
                int num4 = Math.Min(count, (int)VirtualDisk.SectorSize - (int)num3);
                if ((int)_sectorBufferIndex != (int)num2)
                {
                    if (VirtualDisk.SectorIsAllocated(num2))
                    {
                        if (num4 == VirtualDisk.SectorSize)
                        {
                            VirtualDisk.ReadSector(num2, buffer, (uint)offset);
                        }
                        else
                        {
                            VirtualDisk.ReadSector(num2, _sectorBuffer, 0U);
                            _sectorBufferIndex = num2;
                            for (int index = 0; index < num4; ++index)
                            {
                                buffer[offset + index] = _sectorBuffer[num3 + index];
                            }
                        }
                    }
                    else
                    {
                        for (int index = 0; index < num4; ++index)
                        {
                            buffer[offset + index] = 0;
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < num4; ++index)
                    {
                        buffer[offset + index] = _sectorBuffer[num3 + index];
                    }
                }

                offset += num4;
                count -= num4;
                num1 += num4;
                Position += num4;
            }

            return num1;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (offset + Position > Length)
            {
                throw new EndOfStreamException("Cannot write past the end of the stream.");
            }

            while (count > 0)
            {
                uint num1 = (uint)((ulong)Position / VirtualDisk.SectorSize);
                uint num2 = (uint)((ulong)Position % VirtualDisk.SectorSize);
                int num3 = Math.Min(count, (int)VirtualDisk.SectorSize - (int)num2);
                if (!VirtualDisk.SectorIsAllocated(num1))
                {
                    throw new ImageStorageException(
                                        "Writing to an unallocated virtual disk location is not supported.");
                }

                if (num2 == 0U && num3 == VirtualDisk.SectorSize)
                {
                    VirtualDisk.WriteSector(num1, buffer, (uint)offset);
                }
                else
                {
                    if ((int)_sectorBufferIndex != (int)num1)
                    {
                        VirtualDisk.ReadSector(num1, _sectorBuffer, 0U);
                        _sectorBufferIndex = num1;
                    }

                    for (int index = 0; index < num3; ++index)
                    {
                        _sectorBuffer[num2 + index] = buffer[offset + index];
                    }

                    VirtualDisk.WriteSector(num1, _sectorBuffer, 0U);
                }

                offset += num3;
                count -= num3;
                Position += num3;
            }
        }
    }
}