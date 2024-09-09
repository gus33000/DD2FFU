// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.GuidPartitionTableEntry
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class GuidPartitionTableEntry
    {
        private readonly IULogger _logger;
        private byte[] _partitionName;

        public GuidPartitionTableEntry(IULogger logger)
        {
            _logger = logger;
        }

        public Guid PartitionType
        {
            get; set;
        }

        public Guid PartitionId
        {
            get; set;
        }

        public ulong StartingSector
        {
            get; set;
        }

        public ulong LastSector
        {
            get; set;
        }

        public ulong Attributes
        {
            get; set;
        }

        public string PartitionName => Encoding.Unicode.GetString(_partitionName).Split(new char[1])[0];

        public void WriteToStream(Stream stream, int bytesPerEntry)
        {
            BinaryWriter binaryWriter = new(stream);
            long position = stream.Position;
            byte[] byteArray1 = PartitionType.ToByteArray();
            binaryWriter.Write(byteArray1, 0, byteArray1.Length);
            byte[] byteArray2 = PartitionId.ToByteArray();
            binaryWriter.Write(byteArray2, 0, byteArray2.Length);
            binaryWriter.Write(StartingSector);
            binaryWriter.Write(LastSector);
            binaryWriter.Write(Attributes);
            binaryWriter.Write(_partitionName, 0, _partitionName.Length);
            if (stream.Position - position >= bytesPerEntry)
            {
                return;
            }

            int num = (int)(bytesPerEntry - (stream.Position - position));
            for (int index = 0; index < num; ++index)
            {
                stream.WriteByte(0);
            }
        }

        public void ReadFromStream(Stream stream, int entryByteCount)
        {
            BinaryReader binaryReader = new(stream);
            long position = stream.Position;
            PartitionType = new Guid(binaryReader.ReadBytes(16));
            PartitionId = new Guid(binaryReader.ReadBytes(16));
            StartingSector = binaryReader.ReadUInt64();
            LastSector = binaryReader.ReadUInt64();
            Attributes = binaryReader.ReadUInt64();
            _partitionName = binaryReader.ReadBytes(72);
            if (stream.Position - position >= entryByteCount)
            {
                return;
            }

            stream.Position += entryByteCount - (stream.Position - position);
        }

        public void LogInfo(ushort indentLevel = 0)
        {
            if (PartitionType == Guid.Empty || string.IsNullOrEmpty(PartitionName) || StartingSector == 0UL)
            {
                return;
            }

            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            _logger.LogInfo(str + "Partition Name : {0}", PartitionName);
            _logger.LogInfo(str + "Partition Type : {{{0}}}", PartitionType);
            _logger.LogInfo(str + "Partition Id   : {{{0}}}", PartitionId);
            _logger.LogInfo(str + "Starting Sector: 0x{0:x}", StartingSector);
            _logger.LogInfo(str + "Last Sector    : 0x{0:x}", LastSector);
            _logger.LogInfo(str + "Attributes     : 0x{0:x}", Attributes);
            _logger.LogInfo("");
        }

        public void Clean()
        {
            PartitionType = Guid.Empty;
            PartitionId = Guid.Empty;
            StartingSector = 0UL;
            LastSector = 0UL;
            Attributes = 0UL;
            for (int index = 0; index < 72; ++index)
            {
                _partitionName[index] = 0;
            }
        }

        public override string ToString()
        {
            return PartitionName;
        }
    }
}