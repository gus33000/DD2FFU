// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.ValidationEntry
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class ValidationEntry
    {
        private byte[] _compareData;

        public uint SectorIndex
        {
            get; set;
        }

        public int SectorOffset
        {
            get; set;
        }

        public int ByteCount
        {
            get; set;
        }

        public void SetCompareData(byte[] data)
        {
            _compareData = data;
        }

        public byte[] GetCompareData()
        {
            return _compareData;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(SectorIndex);
            writer.Write(SectorOffset);
            writer.Write(ByteCount);
            writer.Write(GetCompareData());
        }

        public void Read(BinaryReader reader)
        {
            SectorIndex = reader.ReadUInt32();
            SectorOffset = reader.ReadInt32();
            ByteCount = reader.ReadInt32();
            SetCompareData(reader.ReadBytes(ByteCount));
        }
    }
}