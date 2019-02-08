// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.DynamicHardDisk
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class DynamicHardDisk : IVirtualHardDisk, IDisposable
    {
        public static byte[] emptySectorBuffer = new byte[(int) VhdCommon.VHDSectorSize];
        private bool _alreadyDisposed;
        private readonly ulong _fileSize;
        private FileStream _fileStream;
        private VhdFooter _footer;
        private ulong _footerOffset;
        private VhdHeader _header;
        private readonly ulong _tableOffset;

        public DynamicHardDisk(string fileName, ulong sectorCount)
        {
            ulong offset1 = 0;
            _fileSize = sectorCount * SectorSize;
            _fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            SectorCount = sectorCount;
            _footer = new VhdFooter(_fileSize, VhdType.Dynamic, (ulong) Marshal.SizeOf(typeof(VhdFooter)));
            WriteVHDFooter(offset1);
            var offset2 = offset1 + (ulong) Marshal.SizeOf(typeof(VhdFooter));
            _header = new VhdHeader(_fileSize);
            WriteVHDHeader(offset2);
            var offset3 = offset2 + (ulong) Marshal.SizeOf(typeof(VhdHeader));
            _tableOffset = offset3;
            AllocationTable = new BlockAllocationTable(_header.MaxTableEntries);
            WriteBlockAllocationTable(offset3);
            var offset4 = offset3 + AllocationTable.SizeInBytes;
            _footerOffset = offset4;
            WriteVHDFooter(offset4);
            var num = offset4 + (ulong) Marshal.SizeOf(typeof(VhdFooter));
        }

        public DynamicHardDisk(string existingFile, bool addWriteAccess = false)
        {
            var access = FileAccess.Read;
            if (addWriteAccess)
                access = FileAccess.ReadWrite;
            _fileStream = new FileStream(existingFile, FileMode.Open, access, FileShare.ReadWrite);
            _footer = VhdFooter.Read(_fileStream);
            _fileSize = _footer.CurrentSize;
            SectorCount = _fileSize / VhdCommon.VHDSectorSize;
            _fileStream.Position = (long) _footer.DataOffset;
            _header = VhdHeader.Read(_fileStream);
            _tableOffset = _header.TableOffset;
            _fileStream.Position = (long) _header.TableOffset;
            AllocationTable = new BlockAllocationTable(_header.MaxTableEntries);
            AllocationTable.Read(_fileStream);
        }

        public BlockAllocationTable AllocationTable { get; }

        public uint BlockSize => _header.BlockSize;

        public uint BlockBitmapSectorCount
        {
            get
            {
                var num1 = (int) (BlockSize / SectorSize);
                var num2 = (uint) num1 / 8U;
                if ((uint) num1 % 8U != 0U)
                    ++num2;
                var num3 = num2 / SectorSize;
                if (num2 % SectorSize != 0U)
                    ++num3;
                return num3;
            }
        }

        private uint SectorsPerBlock => VhdCommon.DynamicVHDBlockSize / SectorSize;

        public uint SectorSize => VhdCommon.VHDSectorSize;

        public ulong SectorCount { get; }

        public void FlushFile()
        {
            WriteVHDFooter(_footerOffset);
            _fileStream.Flush();
        }

        public void ReadSector(ulong sector, byte[] buffer, uint offset)
        {
            if (sector >= SectorCount)
                throw new ArgumentException("Sector is out of bound", nameof(sector));
            if (buffer.Length - offset < SectorSize)
                throw new ArgumentException("The buffer, from the given offset, is smaller than the sector size.",
                    nameof(offset));
            var index = (uint) (sector / SectorsPerBlock);
            var num1 = (uint) (sector % SectorsPerBlock);
            if (uint.MaxValue == AllocationTable[index])
            {
                Array.Copy(emptySectorBuffer, 0L, buffer, offset, SectorSize);
            }
            else
            {
                var buffer1 = new byte[(int) SectorSize];
                _fileStream.Seek((int) AllocationTable[index] * (int) SectorSize, SeekOrigin.Begin);
                _fileStream.Read(buffer1, 0, (int) SectorSize);
                var num2 = (int) (num1 / 8U);
                _fileStream.Seek((AllocationTable[index] + num1 + BlockBitmapSectorCount) * SectorSize,
                    SeekOrigin.Begin);
                _fileStream.Read(buffer, (int) offset, (int) SectorSize);
            }
        }

        public void WriteSector(ulong sector, byte[] buffer, uint offset)
        {
            if (sector >= SectorCount)
                throw new ArgumentException("Sector is out of bound", nameof(sector));
            if (buffer.Length - offset < SectorSize)
                throw new ArgumentException("The buffer, from the given offset, is smaller than the sector size.",
                    nameof(offset));
            var index = (uint) (sector / SectorsPerBlock);
            var num1 = (uint) (sector % SectorsPerBlock);
            if (uint.MaxValue == AllocationTable[index])
            {
                AllocationTable[index] = (uint) (_footerOffset / SectorSize);
                WriteBlockAllocationTable(_tableOffset);
                _fileStream.Seek((long) _footerOffset, SeekOrigin.Begin);
                _fileStream.Write(emptySectorBuffer, 0, (int) SectorSize);
                _footerOffset += SectorSize + VhdCommon.DynamicVHDBlockSize;
            }

            var buffer1 = new byte[(int) SectorSize];
            _fileStream.Seek((int) AllocationTable[index] * (int) SectorSize, SeekOrigin.Begin);
            _fileStream.Read(buffer1, 0, (int) SectorSize);
            var num2 = num1 / 8U;
            var num3 = (byte) (num1 % 8U);
            buffer1[(int) num2] = (byte) (buffer1[(int) num2] | (1U << num3));
            _fileStream.Seek((int) AllocationTable[index] * (int) SectorSize, SeekOrigin.Begin);
            _fileStream.Write(buffer1, 0, (int) SectorSize);
            _fileStream.Seek((uint) ((int) AllocationTable[index] + (int) num1 + 1) * (long) SectorSize,
                SeekOrigin.Begin);
            _fileStream.Write(buffer, (int) offset, (int) SectorSize);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Close()
        {
            if (_fileStream == null)
                return;
            _fileStream.Close();
            _fileStream = null;
        }

        public bool SectorIsAllocated(ulong sectorIndex)
        {
            return uint.MaxValue != AllocationTable[(uint) (sectorIndex / SectorsPerBlock)];
        }

        ~DynamicHardDisk()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadyDisposed)
                return;
            var num = isDisposing ? 1 : 0;
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream = null;
            }

            _alreadyDisposed = true;
        }

        private void WriteVHDFooter(ulong offset)
        {
            _fileStream.Seek((long) offset, SeekOrigin.Begin);
            _footer.Write(_fileStream);
        }

        private void WriteVHDHeader(ulong offset)
        {
            _fileStream.Seek((long) offset, SeekOrigin.Begin);
            _header.Write(_fileStream);
        }

        private void WriteBlockAllocationTable(ulong offset)
        {
            _fileStream.Seek((long) offset, SeekOrigin.Begin);
            AllocationTable.Write(_fileStream);
        }
    }
}