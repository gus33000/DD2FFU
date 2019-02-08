// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.FileIdentifier
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Reflection;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class FileIdentifier : BaseIdentifier, IBlockIoIdentifier, IDeviceIdentifier
    {
        public FileIdentifier(string filePath, BcdElementBootDevice parentDevice)
        {
            Path = filePath;
            ParentDevice = parentDevice;
        }

         public uint Version { get; set; }

        
        public uint Length
        {
            get
            {
                uint num = 12 + 2;
                if (Path != null)
                    num += (uint) (2 * Path.Length);
                if (ParentDevice != null)
                    num += ParentDevice.CalculatedSize;
                return num;
            }
        }

         public uint Type { get; set; }

        public string Path { get; private set; }

        public BcdElementBootDevice ParentDevice { get; set; }

        public void ReadFromStream(BinaryReader reader)
        {
            var position = reader.BaseStream.Position;
            Version = reader.ReadUInt32();
            var num = reader.ReadUInt32();
            Type = reader.ReadUInt32();
            ParentDevice.ReadFromStream(reader);
            if (reader.BaseStream.Position - position >= Length)
                throw new ImageStorageException(string.Format(
                    "{0}: The FileIdentifier appears to be invalid at position: 0x{1:x}  AND  {2] {3} {4}",
                    (object) MethodBase.GetCurrentMethod().Name, (object) position, (object) Version, (object) Length,
                    (object) Type));
            Path = Encoding.Unicode.GetString(
                reader.ReadBytes((int) num - (int) (uint) (reader.BaseStream.Position - position)));
        }

        public void WriteToStream(BinaryWriter writer)
        {
            writer.Write(1U);
            writer.Write(Length);
            writer.Write(5U);
            ParentDevice.WriteToStream(writer);
            foreach (var ch in Path)
                writer.Write((short) ch);
            writer.Write((short) 0);
        }

        
        public void LogInfo(IULogger logger, int indentLevel)
        {
            var str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Block IO Type: File");
            logger.LogInfo(str + "Version:       {0}", (object) Version);
            logger.LogInfo(str + "Length:        {0}", (object) Length);
            logger.LogInfo(str + "Type:          {0}", (object) Type);
            logger.LogInfo(str + "Path:          {0}", (object) Path);
            logger.LogInfo("");
            ParentDevice.LogInfo(logger, checked(indentLevel + 2));
        }

         public uint Size => Length;

         public BlockIoType BlockType => BlockIoType.File;

        
        public void ReplaceParentDeviceIdentifier(IDeviceIdentifier identifier)
        {
            ParentDevice.ReplaceIdentifier(identifier);
        }
    }
}