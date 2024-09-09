// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.RamDiskIdentifier
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class RamDiskIdentifier : BaseIdentifier, IBlockIoIdentifier, IDeviceIdentifier
    {
        public RamDiskIdentifier(string filePath, BcdElementBootDevice parentDevice)
        {
            Source = new FileIdentifier(filePath, parentDevice);
        }

        public ulong ImageBase
        {
            get; set;
        }

        public ulong ImageSize
        {
            get; set;
        }

        public uint ImageOffset
        {
            get; set;
        }

        public FileIdentifier Source
        {
            get; set;
        }

        public void ReadFromStream(BinaryReader reader)
        {
            _ = (int)reader.ReadUInt32();
            ImageBase = reader.ReadUInt32();
            ImageSize = reader.ReadUInt64();
            ImageOffset = reader.ReadUInt32();
            Source.ReadFromStream(reader);
        }

        public void WriteToStream(BinaryWriter writer)
        {
            writer.Write(3UL);
            writer.Write(0U);
            writer.Write(0UL);
            writer.Write(0U);
            Source.WriteToStream(writer);
        }


        public void LogInfo(IULogger logger, int indentLevel)
        {
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Block IO Type: RamDisk");
            logger.LogInfo(str + "ImageBase:     0x{0:x}", ImageBase);
            logger.LogInfo(str + "ImageSize:     0x{0:x}", ImageSize);
            logger.LogInfo(str + "ImageOffset:   0x{0:x}", ImageOffset);
            logger.LogInfo(str + "File Path:     {0}", Source.Path);
            if (Source.ParentDevice == null)
            {
                return;
            }

            Source.ParentDevice.LogInfo(logger, checked(indentLevel + 2));
        }

        public uint Size => Source.Size + 24U;

        public BlockIoType BlockType => BlockIoType.RamDisk;


        public void ReplaceParentDeviceIdentifier(IDeviceIdentifier identifier)
        {
            Source.ReplaceParentDeviceIdentifier(identifier);
        }
    }
}