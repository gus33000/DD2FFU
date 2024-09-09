// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.SerialPortIdentifier
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class SerialPortIdentifier : BaseIdentifier, IDeviceIdentifier
    {
        public uint Type
        {
            get; set;
        }

        public uint PortNumber
        {
            get; set;
        }

        public byte GenericAddressSpaceId
        {
            get; set;
        }

        public byte GenericAddressWidth
        {
            get; set;
        }

        public byte GenericAddressBitOffset
        {
            get; set;
        }

        public byte GenericAddressAccessSize
        {
            get; set;
        }

        public ulong GenericAddressPhysicalAddress
        {
            get; set;
        }

        public void ReadFromStream(BinaryReader reader)
        {
            Type = reader.ReadUInt32();
            byte[] numArray = reader.ReadBytes(12);
            PortNumber = (uint)((numArray[0] << 24) | (numArray[1] << 16) | (numArray[2] << 8)) | numArray[3];
            GenericAddressSpaceId = numArray[0];
            GenericAddressWidth = numArray[1];
            GenericAddressBitOffset = numArray[2];
            GenericAddressAccessSize = numArray[3];
            GenericAddressPhysicalAddress = (ulong)((numArray[4] << 24) | (numArray[5] << 16) | (numArray[6] << 8) |
                                                     numArray[7] | (numArray[8] << 24) | (numArray[9] << 16) |
                                                     (numArray[10] << 8) | numArray[11]);
        }

        public void WriteToStream(BinaryWriter writer)
        {
        }


        public void LogInfo(IULogger logger, int indentLevel)
        {
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Identifier: Serial Port");
        }

        public uint Size => 0;
    }
}