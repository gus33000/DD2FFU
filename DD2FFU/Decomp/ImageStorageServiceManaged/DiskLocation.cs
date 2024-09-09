// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.DiskLocation
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class DiskLocation
    {
        public enum DiskAccessMethod : uint
        {
            DiskBegin = 0,
            DiskEnd = 2
        }

        public DiskLocation()
        {
            BlockIndex = 0U;
            AccessMethod = DiskAccessMethod.DiskBegin;
        }

        public DiskLocation(uint blockIndex)
        {
            BlockIndex = blockIndex;
            AccessMethod = DiskAccessMethod.DiskBegin;
        }

        public DiskLocation(uint blockIndex, DiskAccessMethod accessMethod)
        {
            BlockIndex = blockIndex;
            AccessMethod = accessMethod;
        }

        public DiskAccessMethod AccessMethod
        {
            set; get;
        }

        public uint BlockIndex
        {
            set; get;
        }

        public static int SizeInBytes => 8;

        public void Write(BinaryWriter writer)
        {
            writer.Write((uint)AccessMethod);
            writer.Write(BlockIndex);
        }

        public void Read(BinaryReader reader)
        {
            AccessMethod = (DiskAccessMethod)reader.ReadUInt32();
            BlockIndex = reader.ReadUInt32();
        }

        public void LogInfo(IULogger logger, ushort indentLevel = 0)
        {
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Access Method: {0}", AccessMethod);
            logger.LogInfo(str + "Block Index  : {0}", BlockIndex);
            logger.LogInfo("");
        }
    }
}