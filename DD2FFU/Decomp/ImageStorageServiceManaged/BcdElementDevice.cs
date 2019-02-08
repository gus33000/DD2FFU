// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementDevice
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
    public class BcdElementDevice : BcdElement
    {
        public BcdElementDevice(byte[] binaryData, BcdElementDataType dataType)
            : base(dataType)
        {
            SetBinaryData(binaryData);
            ReadFromStream(new MemoryStream(binaryData));
        }

        public BcdElementDevice(BcdElementBootDevice bootDevice, BcdElementDataType dataType)
            : base(dataType)
        {
            AdditionalFlags = Guid.Empty;
            BootDevice = bootDevice;
            var numArray = new byte[(int) BinarySize];
            using (var memoryStream = new MemoryStream(numArray))
            {
                WriteToStream(memoryStream);
                SetBinaryData(numArray);
            }
        }

        public BcdElementDevice(BcdElementBootDevice bootDevice, BcdElementDataType dataType, Guid Flags)
            : base(dataType)
        {
            AdditionalFlags = Flags;
            BootDevice = bootDevice;
            using (var memoryStream = new MemoryStream())
            {
                WriteToStream(memoryStream);
                SetBinaryData(memoryStream.ToArray());
            }
        }

        public Guid AdditionalFlags { get; set; }

        public BcdElementBootDevice BootDevice { get; set; }

         public uint BinarySize => 16U + BootDevice.Size;

        public static BcdElementDevice CreateBaseBootDevice()
        {
            return new BcdElementDevice(BcdElementBootDevice.CreateBaseBootDevice(),
                new BcdElementDataType(285212673U));
        }

        public static BcdElementDevice CreateBaseRamdiskDevice(string filePath, BcdElementBootDevice parentDevice)
        {
            return new BcdElementDevice(BcdElementBootDevice.CreateBaseRamdiskDevice(filePath, parentDevice),
                BcdElementDataTypes.OsLoaderDevice, BcdObjects.WindowsSetupRamdiskOptions);
        }

        public void ReadFromStream(Stream stream)
        {
            var reader = new BinaryReader(stream);
            AdditionalFlags = new Guid(reader.ReadBytes(16));
            BootDevice = new BcdElementBootDevice();
            BootDevice.ReadFromStream(reader);
        }

        public void WriteToStream(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            var byteArray = AdditionalFlags.ToByteArray();
            writer.Write(byteArray);
            BootDevice.WriteToStream(writer);
        }

        
        public override void LogInfo(IULogger logger, int indentLevel)
        {
            var str = new StringBuilder().Append(' ', indentLevel).ToString();
            base.LogInfo(logger, indentLevel);
            logger.LogInfo(str + "Additional Flags:   {{{0}}}", (object) AdditionalFlags);
            logger.LogInfo("");
            BootDevice.LogInfo(logger, checked(indentLevel + 2));
        }

        
        public void ReplaceRamDiskDeviceIdentifier(IDeviceIdentifier identifier)
        {
            var identifier1 = (RamDiskIdentifier) BootDevice.Identifier;
            if (identifier1 == null)
                throw new ImageStorageException(string.Format("{0}: The device's identifier is not a ramdisk.",
                    MethodBase.GetCurrentMethod().Name));
            identifier1.ReplaceParentDeviceIdentifier(identifier);
            BootDevice.ReplaceIdentifier(identifier1);
            var numArray = new byte[(int) BinarySize];
            using (var memoryStream = new MemoryStream(numArray))
            {
                WriteToStream(memoryStream);
                memoryStream.Flush();
                memoryStream.Close();
            }

            SetBinaryData(numArray);
        }

        
        public void ReplaceBootDeviceIdentifier(IDeviceIdentifier identifier)
        {
            BootDevice.ReplaceIdentifier(identifier);
            var numArray = new byte[(int) BinarySize];
            var memoryStream = new MemoryStream(numArray);
            WriteToStream(memoryStream);
            SetBinaryData(numArray);
            memoryStream.Close();
        }
    }
}