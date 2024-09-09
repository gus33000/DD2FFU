// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.MetadataPartitionHeader
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class MetadataPartitionHeader
    {
        public uint Signature
        {
            get; private set;
        }

        public uint MaxPartitionCount
        {
            get; private set;
        }

        public uint PartitionCount
        {
            get; private set;
        }

        public void ReadFromStream(BinaryReader reader)
        {
            Signature = reader.ReadUInt32();
            MaxPartitionCount = reader.ReadUInt32();
            PartitionCount = reader.ReadUInt32();
        }

        public void LogInfo(IULogger logger, ushort indentLevel = 0)
        {
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Signature          : {0}", Signature);
            logger.LogInfo(str + "Max Partition Count: {0}", MaxPartitionCount);
            logger.LogInfo(str + "Partition Count    : {0}", PartitionCount);
        }
    }
}