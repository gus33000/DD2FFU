// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.OEMDevicePlatformInput
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ImageCommon.dll

using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    [XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    [XmlRoot(ElementName = "OEMDevicePlatform", IsNullable = false,
        Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public class OEMDevicePlatformInput
    {
        private string[] _idArray;

        public uint AdditionalMainOSFreeSectorsRequest;

        [XmlElement("BIOSMode")] public string BIOSMode = "UEFI";

        [XmlArrayItem(ElementName = "Name")]
        [XmlArray("CompressedPartitions")]
        public string[] CompressedPartitions;

        [XmlElement("DevicePlatformID")] public string DevicePlatformID;

        [XmlElement("MainOSRTCDataReservedSectors")]
        public uint MainOSRTCDataReservedSectors;

        public uint MinSectorCount;

        public uint MMOSPartitionTotalSectorsOverride;

        [XmlElement("Rules")] public InputRules Rules;

        [XmlArrayItem(ElementName = "Name")]
        [XmlArray("UncompressedPartitions")]
        public string[] UncompressedPartitions;

        [XmlArrayItem(ElementName = "ID")]
        [XmlArray("DevicePlatformIDs")]
        public string[] DevicePlatformIDs
        {
            get => DevicePlatformID != null && _idArray != null
                    ? throw new ImageCommonException(
                                        "Please specify either a DevicePlatformID or a group of DevicePlatformIDs in the device platform package, but not both.")
                    : DevicePlatformID == null && _idArray == null
                    ? throw new ImageCommonException(
                                        "Please specify either a DevicePlatformID or a group of DevicePlatformIDs in the device platform package. No platform ID is currently present.")
                    : DevicePlatformID == null ? _idArray : (new string[1] { DevicePlatformID });
            set => _idArray = value;
        }
    }
}