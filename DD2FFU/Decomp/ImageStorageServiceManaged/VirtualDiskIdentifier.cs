// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.VirtualDiskIdentifier
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
    public class VirtualDiskIdentifier : BaseIdentifier, IBlockIoIdentifier, IDeviceIdentifier
    {
        public HardDiskIdentifier InternalIdentifer { get; set; }

        public BcdElementBootDevice FileDevice { get; set; }

        public void ReadFromStream(BinaryReader reader)
        {
            InternalIdentifer = new HardDiskIdentifier();
            InternalIdentifer.ReadFromStream(reader);
            InternalIdentifer.AsVirtualDisk = true;
            FileDevice = new BcdElementBootDevice();
            FileDevice.ReadFromStream(reader);
        }

        public void WriteToStream(BinaryWriter writer)
        {
            throw new ImageStorageException(string.Format("{0}: This function isn't implemented.",
                MethodBase.GetCurrentMethod().Name));
        }

        
        public void LogInfo(IULogger logger, int indentLevel)
        {
            var str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Identifier: Virtual Hard Disk");
            InternalIdentifer.LogInfo(logger, checked(indentLevel + 2));
            logger.LogInfo("");
            FileDevice.LogInfo(logger, checked(indentLevel + 2));
        }

         public BlockIoType BlockType => BlockIoType.VirtualHardDisk;

         public uint Size => InternalIdentifer.Size + FileDevice.Size;
    }
}