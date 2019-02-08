// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementDeviceInput
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementDeviceInput
    {
        [XmlIgnore] public DeviceTypeChoice DeviceType;

        [XmlChoiceIdentifier("DeviceType")]
        [XmlElement("GPTDevice", typeof(BcdElementDeviceGptInput))]
        [XmlElement("MBRDevice", typeof(BcdElementDeviceMbrInput))]
        [XmlElement("RamdiskDevice", typeof(BcdElementDeviceRamdiskInput))]
        public object DeviceValue { get; set; }

        public void SaveAsRegFile(TextWriter writer, string elementName)
        {
            switch (DeviceType)
            {
                case DeviceTypeChoice.GPTDevice:
                    var gptBootDevice =
                        BcdElementDeviceGptInput.CreateGptBootDevice(DeviceValue as BcdElementDeviceGptInput);
                    var buffer1 = new byte[(int) gptBootDevice.BinarySize];
                    var memoryStream1 = new MemoryStream(buffer1);
                    try
                    {
                        gptBootDevice.WriteToStream(memoryStream1);
                    }
                    finally
                    {
                        memoryStream1.Flush();
                    }

                    BcdElementValueTypeInput.WriteByteArray(writer, elementName, "\"Element\"=hex:", buffer1);
                    writer.WriteLine();
                    break;
                case DeviceTypeChoice.MBRDevice:
                    var baseBootDevice = BcdElementDevice.CreateBaseBootDevice();
                    var buffer2 = new byte[(int) baseBootDevice.BinarySize];
                    var memoryStream2 = new MemoryStream(buffer2);
                    try
                    {
                        baseBootDevice.WriteToStream(memoryStream2);
                    }
                    finally
                    {
                        memoryStream2.Flush();
                    }

                    BcdElementValueTypeInput.WriteByteArray(writer, elementName, "\"Element\"=hex:", buffer2);
                    writer.WriteLine();
                    break;
                case DeviceTypeChoice.RamdiskDevice:
                    var deviceValue = DeviceValue as BcdElementDeviceRamdiskInput;
                    BcdElementBootDevice parentDevice;
                    switch (deviceValue.ParentDevice.DeviceType)
                    {
                        case DeviceTypeChoice.GPTDevice:
                            parentDevice = BcdElementDeviceGptInput
                                .CreateGptBootDevice(deviceValue.ParentDevice.DeviceValue as BcdElementDeviceGptInput)
                                .BootDevice;
                            break;
                        case DeviceTypeChoice.MBRDevice:
                            parentDevice = BcdElementBootDevice.CreateBaseBootDevice();
                            break;
                        default:
                            throw new ImageStorageException(string.Format(
                                "{0}: The given Ramdisk parent type is not supported.",
                                MethodBase.GetCurrentMethod().Name));
                    }

                    var baseRamdiskDevice =
                        BcdElementDevice.CreateBaseRamdiskDevice(deviceValue.FilePath, parentDevice);
                    var memoryStream3 = new MemoryStream();
                    byte[] numArray;
                    try
                    {
                        baseRamdiskDevice.WriteToStream(memoryStream3);
                        numArray = new byte[memoryStream3.ToArray().Length];
                        Array.Copy(memoryStream3.ToArray(), numArray, memoryStream3.Length);
                    }
                    finally
                    {
                        memoryStream3.Flush();
                    }

                    BcdElementValueTypeInput.WriteByteArray(writer, elementName, "\"Element\"=hex:", numArray);
                    writer.WriteLine();
                    break;
                default:
                    throw new ImageStorageException(string.Format("{0}: Unsupported partition type: {1}.",
                        MethodBase.GetCurrentMethod().Name, DeviceType));
            }
        }

        public void SaveAsRegData(BcdRegData bcdRegData, string path)
        {
            switch (DeviceType)
            {
                case DeviceTypeChoice.GPTDevice:
                    var gptBootDevice =
                        BcdElementDeviceGptInput.CreateGptBootDevice(DeviceValue as BcdElementDeviceGptInput);
                    var buffer1 = new byte[(int) gptBootDevice.BinarySize];
                    var memoryStream1 = new MemoryStream(buffer1);
                    try
                    {
                        gptBootDevice.WriteToStream(memoryStream1);
                    }
                    finally
                    {
                        memoryStream1.Flush();
                    }

                    BcdElementValueTypeInput.WriteByteArray(bcdRegData, path, buffer1);
                    break;
                case DeviceTypeChoice.MBRDevice:
                    var baseBootDevice = BcdElementDevice.CreateBaseBootDevice();
                    var buffer2 = new byte[(int) baseBootDevice.BinarySize];
                    var memoryStream2 = new MemoryStream(buffer2);
                    try
                    {
                        baseBootDevice.WriteToStream(memoryStream2);
                    }
                    finally
                    {
                        memoryStream2.Flush();
                    }

                    BcdElementValueTypeInput.WriteByteArray(bcdRegData, path, buffer2);
                    break;
                case DeviceTypeChoice.RamdiskDevice:
                    var deviceValue = DeviceValue as BcdElementDeviceRamdiskInput;
                    BcdElementBootDevice parentDevice;
                    switch (deviceValue.ParentDevice.DeviceType)
                    {
                        case DeviceTypeChoice.GPTDevice:
                            parentDevice = BcdElementDeviceGptInput
                                .CreateGptBootDevice(deviceValue.ParentDevice.DeviceValue as BcdElementDeviceGptInput)
                                .BootDevice;
                            break;
                        case DeviceTypeChoice.MBRDevice:
                            parentDevice = BcdElementBootDevice.CreateBaseBootDevice();
                            break;
                        default:
                            throw new ImageStorageException(string.Format(
                                "{0}: The given Ramdisk parent type is not supported.",
                                MethodBase.GetCurrentMethod().Name));
                    }

                    var baseRamdiskDevice =
                        BcdElementDevice.CreateBaseRamdiskDevice(deviceValue.FilePath, parentDevice);
                    var memoryStream3 = new MemoryStream();
                    byte[] numArray;
                    try
                    {
                        baseRamdiskDevice.WriteToStream(memoryStream3);
                        numArray = new byte[memoryStream3.ToArray().Length];
                        Array.Copy(memoryStream3.ToArray(), numArray, memoryStream3.Length);
                    }
                    finally
                    {
                        memoryStream3.Flush();
                    }

                    BcdElementValueTypeInput.WriteByteArray(bcdRegData, path, numArray);
                    break;
                default:
                    throw new ImageStorageException(string.Format("{0}: Unsupported partition type: {1}.",
                        MethodBase.GetCurrentMethod().Name, DeviceType));
            }
        }
    }
}