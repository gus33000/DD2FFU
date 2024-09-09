﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.CdRomIdentifier
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Reflection;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class CdRomIdentifier : BaseIdentifier, IBlockIoIdentifier, IDeviceIdentifier
    {
        public uint CdRomNumber
        {
            get; set;
        }

        public void ReadFromStream(BinaryReader reader)
        {
            CdRomNumber = reader.ReadUInt32();
        }

        public void WriteToStream(BinaryWriter writer)
        {
            throw new ImageStorageException(string.Format("{0}: This function isn't implemented.",
                MethodBase.GetCurrentMethod().Name));
        }


        public void LogInfo(IULogger logger, int indentLevel)
        {
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Identifier: CdRom");
            logger.LogInfo(str + "CdRom Number:  {0}", CdRomNumber);
        }

        public uint Size => 0;

        public BlockIoType BlockType => BlockIoType.HardDisk;
    }
}