// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.MetadataPartitionEntry
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class MetadataPartitionEntry
    {
        public static int PartitionNameLength = 36;
        public static int BytesPerGuid = 16;

        public Guid PartitionId
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public ulong DiskOffset
        {
            get; set;
        }

        public void ReadFromStream(BinaryReader reader)
        {
            DiskOffset = reader.ReadUInt64();
            string str = Encoding.Unicode.GetString(reader.ReadBytes(PartitionNameLength * 2));
            Name = str[..str.IndexOf(char.MinValue)];
            PartitionId = new Guid(reader.ReadBytes(BytesPerGuid));
        }

        public void LogInfo(IULogger logger, ushort indentLevel = 0)
        {
            _ = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo((int)indentLevel + "Name        : {0}", Name);
            logger.LogInfo((int)indentLevel + "Partition Id: {0}", PartitionId);
            logger.LogInfo((int)indentLevel + "Disk Offset : {0}", DiskOffset);
        }
    }
}