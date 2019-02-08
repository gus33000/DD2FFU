// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.MbrPartitionEntry
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class MbrPartitionEntry
    {
        public const int SizeInBytes = 16;

        public bool Bootable { get; set; }

        public byte PartitionType { get; set; }

        public uint StartingSector { get; set; }

        public uint SectorCount { get; set; }

        public bool TypeIsContainer
        {
            get
            {
                if (PartitionType != 5)
                    return PartitionType == 19;
                return true;
            }
        }

        public uint StartingSectorOffset { get; set; }

        public uint AbsoluteStartingSector => StartingSectorOffset + StartingSector;

        public void ReadFromStream(BinaryReader reader)
        {
            Bootable = (reader.ReadByte() & 128U) > 0U;
            reader.ReadBytes(3);
            PartitionType = reader.ReadByte();
            reader.ReadBytes(3);
            StartingSector = reader.ReadUInt32();
            SectorCount = reader.ReadUInt32();
        }

        public void WriteToStream(BinaryWriter writer)
        {
            writer.Write(Bootable ? (byte) 128 : (byte) 0);
            writer.Write((byte) 0);
            writer.Write((byte) 0);
            writer.Write((byte) 0);
            writer.Write(PartitionType);
            writer.Write((byte) 0);
            writer.Write((byte) 0);
            writer.Write((byte) 0);
            writer.Write(StartingSector);
            writer.Write(SectorCount);
        }

        public void ZeroData()
        {
            PartitionType = 0;
            StartingSector = 0U;
            StartingSectorOffset = 0U;
            SectorCount = 0U;
        }

        public void LogInfo(IULogger logger, MasterBootRecord masterBootRecord, ushort indentLevel = 0)
        {
            if (StartingSector == 0U || SectorCount == 0U || PartitionType == 0)
                return;
            var str1 = new StringBuilder().Append(' ', indentLevel).ToString();
            var str2 = "<EBR>";
            if (!TypeIsContainer)
                str2 = masterBootRecord.GetPartitionName(this);
            logger.LogInfo(str1 + "Partition Name : {0}", (object) str2);
            logger.LogInfo(str1 + "Partition Type : 0x{0:x}", (object) PartitionType);
            logger.LogInfo(str1 + "Starting Sector: 0x{0:x}", (object) StartingSector);
            logger.LogInfo(str1 + "Sector Count   : 0x{0:x}", (object) SectorCount);
            if (masterBootRecord.IsExtendedBootRecord())
                logger.LogInfo(str1 + "Absolute Starting Sector: 0x{0:x}", (object) AbsoluteStartingSector);
            logger.LogInfo("");
        }
    }
}