// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.GuidPartitionTableHeader
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class GuidPartitionTableHeader
    {
        public const ulong HeaderSignature = 6075990659671082565;
        private readonly IULogger _logger;

        public GuidPartitionTableHeader(IULogger logger)
        {
            _logger = logger;
        }

        public ulong Signature
        {
            get; set;
        }

        public uint Revision
        {
            get; set;
        }

        public uint HeaderSize
        {
            get; set;
        }

        public uint HeaderCrc32
        {
            get; set;
        }

        public uint Reserved
        {
            get; set;
        }

        public ulong HeaderSector
        {
            get; set;
        }

        public ulong AlternateHeaderSector
        {
            get; set;
        }

        public ulong FirstUsableSector
        {
            get; set;
        }

        public ulong LastUsableSector
        {
            get; set;
        }

        public Guid DiskId
        {
            get; set;
        }

        public ulong PartitionEntryStartSector
        {
            get; set;
        }

        public uint PartitionEntryCount
        {
            get; set;
        }

        public uint PartitionEntrySizeInBytes
        {
            get; set;
        }

        public uint PartitionEntryArrayCrc32
        {
            get; set;
        }

        public void WriteToStream(Stream stream, int bytesPerSector)
        {
            BinaryWriter binaryWriter = new(stream);
            binaryWriter.Write(Signature);
            binaryWriter.Write(Revision);
            binaryWriter.Write(HeaderSize);
            binaryWriter.Write(HeaderCrc32);
            binaryWriter.Write(Reserved);
            binaryWriter.Write(HeaderSector);
            binaryWriter.Write(AlternateHeaderSector);
            binaryWriter.Write(FirstUsableSector);
            binaryWriter.Write(LastUsableSector);
            byte[] byteArray = DiskId.ToByteArray();
            binaryWriter.Write(byteArray, 0, byteArray.Length);
            binaryWriter.Write(PartitionEntryStartSector);
            binaryWriter.Write(PartitionEntryCount);
            binaryWriter.Write(PartitionEntrySizeInBytes);
            binaryWriter.Write(PartitionEntryArrayCrc32);
            if (stream.Position % bytesPerSector == 0L)
            {
                return;
            }

            stream.Position += bytesPerSector - (stream.Position % bytesPerSector);
        }

        public void ReadFromStream(Stream stream, int bytesPerSector)
        {
            BinaryReader binaryReader = new(stream);
            Signature = binaryReader.ReadUInt64();
            if (Signature != 6075990659671082565UL)
            {
                throw new ImageStorageException("The EFI header signature is invalid.");
            }

            Revision = binaryReader.ReadUInt32();
            if (Revision != 65536U)
            {
                throw new ImageStorageException("The EFI header revision is an unsupported version.");
            }

            HeaderSize = binaryReader.ReadUInt32();
            HeaderCrc32 = binaryReader.ReadUInt32();
            Reserved = binaryReader.ReadUInt32();
            if (Reserved != 0U)
            {
                throw new ImageStorageException("The reserved field in the EFI header is not zero.");
            }

            HeaderSector = binaryReader.ReadUInt64();
            AlternateHeaderSector = binaryReader.ReadUInt64();
            FirstUsableSector = binaryReader.ReadUInt64();
            LastUsableSector = binaryReader.ReadUInt64();
            DiskId = new Guid(binaryReader.ReadBytes(16));
            PartitionEntryStartSector = binaryReader.ReadUInt64();
            PartitionEntryCount = binaryReader.ReadUInt32();
            PartitionEntrySizeInBytes = binaryReader.ReadUInt32();
            PartitionEntryArrayCrc32 = binaryReader.ReadUInt32();
            if (stream.Position % bytesPerSector == 0L)
            {
                return;
            }

            stream.Position += bytesPerSector - (stream.Position % bytesPerSector);
        }

        public void LogInfo(ushort indentLevel = 0)
        {
            _logger.LogInfo(new StringBuilder().Append(' ', indentLevel) + "GUID Partition Table Header");
            indentLevel += 2;
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            _logger.LogInfo(str + "Revision                     : 0x{0:x}", Revision);
            _logger.LogInfo(str + "Header Size                  : 0x{0:x}", HeaderSize);
            _logger.LogInfo(str + "Header Sector                : 0x{0:x}", HeaderSector);
            _logger.LogInfo(str + "Alternate Header Sector      : 0x{0:x}", AlternateHeaderSector);
            _logger.LogInfo(str + "First Usable Sector          : 0x{0:x}", FirstUsableSector);
            _logger.LogInfo(str + "Last Usable Sector           : 0x{0:x}", LastUsableSector);
            _logger.LogInfo(str + "Disk Id                      : {{{0}}}", DiskId);
            _logger.LogInfo(str + "Partition Entry Start Sector : 0x{0:x}", PartitionEntryStartSector);
            _logger.LogInfo(str + "Partition Entry Size In Bytes: 0x{0:x}", PartitionEntrySizeInBytes);
            _logger.LogInfo(str + "Partition Entry Array CRC    : 0x{0:x}", PartitionEntryArrayCrc32);
        }

        public bool IsValid(ulong headerSectorIndex, byte[] partitionEntryArray)
        {
            return false;
        }

        private uint ComputeHeaderCrc(int bytesPerSector)
        {
            MemoryStream memoryStream = new();
            CRC32 crC32 = new();
            uint headerCrc32 = HeaderCrc32;
            HeaderCrc32 = 0U;
            WriteToStream(memoryStream, bytesPerSector);
            byte[] buffer = memoryStream.GetBuffer();
            int offset = 0;
            int headerSize = (int)HeaderSize;
            byte[] hash = crC32.ComputeHash(buffer, offset, headerSize);
            int num = (hash[0] << 24) | (hash[1] << 16) | (hash[2] << 8) | hash[3];
            HeaderCrc32 = headerCrc32;
            return (uint)num;
        }

        public void ValidateHeaderCrc(int bytesPerSector)
        {
            uint headerCrc = ComputeHeaderCrc(bytesPerSector);
            if ((int)HeaderCrc32 != (int)headerCrc)
            {
                throw new ImageStorageException(string.Format(
                                "The GPT header CRC is invalid.  Actual: {0:x} Expected {1:x}.", HeaderCrc32, headerCrc));
            }
        }

        public void FixHeaderCrc(int bytesPerSector)
        {
            HeaderCrc32 = ComputeHeaderCrc(bytesPerSector);
        }
    }
}