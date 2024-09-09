// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.InputPartition
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ImageCommon.dll

using System.Globalization;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class InputPartition
    {
        private string _primaryPartition;

        public uint GeneratedFileOverheadSectors;

        public uint MinFreeSectors;

        public string Name
        {
            get; set;
        }

        public string Type
        {
            get; set;
        }

        public string Id
        {
            get; set;
        }

        public bool ReadOnly
        {
            get; set;
        }

        public bool AttachDriveLetter
        {
            get; set;
        }

        public bool Hidden
        {
            get; set;
        }

        public bool Bootable
        {
            get; set;
        }

        public uint TotalSectors
        {
            get; set;
        }

        public bool UseAllSpace
        {
            get; set;
        }

        public string FileSystem
        {
            get; set;
        }

        public string UpdateType
        {
            get; set;
        }

        public bool Compressed
        {
            get; set;
        }

        [XmlElement("RequiresCompression")]
        public bool RequiresCompression
        {
            get; set;
        }

        public string PrimaryPartition
        {
            set => _primaryPartition = value;
            get => string.IsNullOrEmpty(_primaryPartition) ? Name : _primaryPartition;
        }

        public bool RequiredToFlash
        {
            get; set;
        }

        public bool SingleSectorAlignment
        {
            get; set;
        }

        [XmlIgnore]
        public uint ByteAlignment
        {
            get; set;
        }

        [XmlElement("ByteAlignment")]
        public string ByteAlignmentString
        {
            get => ByteAlignment.ToString(CultureInfo.InvariantCulture);
            set
            {
                if (!InputHelpers.StringToUint(value, out uint num))
                {
                    throw new ImageCommonException(string.Format("Partition {0}'s byte alignment cannot be parsed.",
                                        string.IsNullOrEmpty(Name) ? "Unknown" : Name));
                }

                ByteAlignment = num;
            }
        }

        [XmlIgnore]
        public uint ClusterSize
        {
            get; set;
        }

        [XmlElement("ClusterSize")]
        public string ClusterSizeString
        {
            get => ClusterSize.ToString(CultureInfo.InvariantCulture);
            set
            {
                if (!InputHelpers.StringToUint(value, out uint num))
                {
                    throw new ImageCommonException(string.Format("Partition {0}'s cluster size cannot be parsed.",
                                        string.IsNullOrEmpty(Name) ? "Unknown" : Name));
                }

                ClusterSize = num;
            }
        }

        [XmlIgnore]
        public ulong OffsetInSectors
        {
            get; set;
        }

        [XmlElement("OffsetInSectors")]
        public string OffsetInSectorsString
        {
            get => OffsetInSectors.ToString(CultureInfo.InvariantCulture);
            set
            {
                if (!InputHelpers.StringToUint64(value, out ulong num))
                {
                    throw new ImageCommonException(string.Format("Partition {0}'s OffsetInSectors cannot be parsed.",
                                        string.IsNullOrEmpty(Name) ? "Unknown" : Name));
                }

                OffsetInSectors = num;
            }
        }
    }
}