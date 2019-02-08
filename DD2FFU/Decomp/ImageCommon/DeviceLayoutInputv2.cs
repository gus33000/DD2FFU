// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.DeviceLayoutInputv2
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ImageCommon.dll

using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    [XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate/v2")]
    [XmlRoot(ElementName = "DeviceLayout", IsNullable = false,
        Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate/v2")]
    public class DeviceLayoutInputv2
    {
        [XmlArrayItem(ElementName = "Store", IsNullable = false, Type = typeof(InputStore))] [XmlArray]
        public InputStore[] Stores;

        [XmlElement("SectorSize")] public uint SectorSize { get; set; }

        [XmlElement("ChunkSize")] public uint ChunkSize { get; set; } = 256;

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

        public InputStore MainOSStore
        {
            get { return Stores.FirstOrDefault(x => x.IsMainOSStore()); }
        }
    }
}