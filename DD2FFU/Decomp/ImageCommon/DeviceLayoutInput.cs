// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.DeviceLayoutInput
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ImageCommon.dll

using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    [XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    [XmlRoot(ElementName = "DeviceLayout", IsNullable = false,
        Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public class DeviceLayoutInput
    {
        [XmlArrayItem(ElementName = "Partition", IsNullable = false, Type = typeof(InputPartition))] [XmlArray]
        public InputPartition[] Partitions;

        [XmlElement("SectorSize")]
        
        public uint SectorSize { get; set; }

        [XmlElement("ChunkSize")]
        
        public uint ChunkSize { get; set; } = 256;

        [XmlIgnore] public uint DefaultPartitionByteAlignment { get; set; }

        [XmlElement("VersionTag")] public string VersionTag { get; set; }

        [XmlElement("DefaultPartitionByteAlignment")]
        public string DefaultPartitionByteAlignmentAsString
        {
            get => DefaultPartitionByteAlignment.ToString(CultureInfo.InvariantCulture);
            set
            {
                uint num = 0;
                if (!InputHelpers.StringToUint(value, out num))
                    throw new ImageCommonException(string.Format("The default byte alignment cannot be parsed: {0}",
                        value));
                DefaultPartitionByteAlignment = num;
            }
        }
    }
}