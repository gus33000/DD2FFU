// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementBootDevice
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Reflection;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementBootDevice
    {

        public enum DeviceType : uint
        {
            BlockIo = 0,
            Unused = 1,
            Partition = 2,
            SerialPort = 3,
            Udp = 4,
            Boot = 5,
            PartitionEx = 6,
            Locate = 8
        }

        public static readonly uint BaseBootDeviceSizeInBytes = 72;

        public DeviceType Type
        {
            get; set;
        }

        public uint Flags
        {
            get; set;
        }

        internal static uint BaseSize => 16;

        public uint Size
        {
            get; set;
        }


        public uint CalculatedSize
        {
            get
            {
                uint baseSize = BaseSize;
                if (Identifier != null)
                {
                    baseSize += Identifier.Size;
                }

                return baseSize;
            }
        }

        public IDeviceIdentifier Identifier
        {
            get; protected set;
        }

        internal long OriginalStreamPosition
        {
            get; set;
        }

        public static BcdElementBootDevice CreateBaseBootDevice()
        {
            return new BcdElementBootDevice
            {
                Type = DeviceType.Boot,
                Size = BaseBootDeviceSizeInBytes,
                Flags = 0,
                Identifier = new BootIdentifier()
            };
        }

        public static BcdElementBootDevice CreateBaseRamdiskDevice(string filePath, BcdElementBootDevice parentDevice)
        {
            return new BcdElementBootDevice
            {
                Type = DeviceType.BlockIo,
                Size = 33,
                Flags = 1,
                Identifier = new RamDiskIdentifier(filePath, parentDevice)
            };
        }


        public void ReplaceIdentifier(IDeviceIdentifier identifier)
        {
            Identifier = identifier;
            if (Identifier.GetType() == typeof(PartitionIdentifierEx))
            {
                Type = DeviceType.PartitionEx;
            }

            Size = BaseSize + identifier.Size;
        }

        public void ReadFromStream(BinaryReader reader)
        {
            OriginalStreamPosition = reader.BaseStream.Position;
            Type = (DeviceType)reader.ReadUInt32();
            Flags = reader.ReadUInt32();
            Size = reader.ReadUInt32();

            _ = (int)reader.ReadUInt32();
            switch (Type)
            {
                case DeviceType.BlockIo:
                    Identifier = BlockIoIdentifierFactory.CreateFromStream(reader);
                    goto case DeviceType.Boot;
                case DeviceType.Partition:
                    Identifier = new PartitionIdentifier();
                    goto case DeviceType.Boot;
                case DeviceType.SerialPort:
                    Identifier = new SerialPortIdentifier();
                    goto case DeviceType.Boot;
                case DeviceType.Udp:
                    Identifier = new UdpIdentifier();
                    goto case DeviceType.Boot;
                case DeviceType.Boot:
                    if (Identifier == null)
                    {
                        break;
                    }

                    Identifier.Parent = this;
                    Identifier.ReadFromStream(reader);
                    if (reader.BaseStream.Position - OriginalStreamPosition >= Size)
                    {
                        break;
                    }

                    uint num2 = Size - (uint)(reader.BaseStream.Position - OriginalStreamPosition);
                    foreach (byte readByte in reader.ReadBytes((int)num2))
                    {
                        if (readByte != 0)
                        {
                            throw new ImageStorageException(string.Format(
                                                        "{0}: Non-zero data was found at the end of a boot device object.",
                                                        MethodBase.GetCurrentMethod().Name));
                        }
                    }

                    break;
                case DeviceType.PartitionEx:
                    Identifier = new PartitionIdentifierEx();
                    goto case DeviceType.Boot;
                case DeviceType.Locate:
                    Identifier = new LocateIdentifier();
                    goto case DeviceType.Boot;
                default:
                    throw new ImageStorageException("Unknown Device Identifier type.");
            }
        }

        public void WriteToStream(BinaryWriter writer)
        {
            writer.Write((uint)Type);
            writer.Write(Flags);
            writer.Write(BaseSize + Identifier.Size);
            writer.Write(0U);
            Identifier.WriteToStream(writer);
        }


        public void LogInfo(IULogger logger, int indentLevel)
        {
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + string.Format("Boot Device:  {0}", Type));
            logger.LogInfo(str + string.Format("Device Flags: 0x{0:x}", Flags));
            logger.LogInfo(str + string.Format("Device Size:  0x{0:x}", Size));
            if (Identifier == null)
            {
                return;
            }

            logger.LogInfo("");
            Identifier.LogInfo(logger, checked(indentLevel + 2));
        }
    }
}