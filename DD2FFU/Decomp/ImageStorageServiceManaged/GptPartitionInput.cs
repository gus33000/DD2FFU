// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.GptPartitionInput
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class GptPartitionInput
    {
        [XmlChoiceIdentifier("PartitionSpecifier")]
        [XmlElement("Name", typeof(string))]
        [XmlElement("Id", typeof(string))]
        public object DataType
        {
            get; set;
        }

        [XmlIgnore]
        public GptPartitionTypeChoice PartitionSpecifier
        {
            get; set;
        }

        [XmlIgnore]
        public Guid PartitionId
        {
            get
            {
                Guid guid;
                if (PartitionSpecifier == GptPartitionTypeChoice.Id)
                {
                    guid = new Guid(DataType as string);
                }
                else
                {
                    string dataType = DataType as string;
                    if (string.Compare(ImageConstants.MAINOS_PARTITION_NAME, dataType, true,
                            CultureInfo.InvariantCulture) == 0)
                    {
                        guid = ImageConstants.MAINOS_PARTITION_ID;
                    }
                    else if (string.Compare(ImageConstants.SYSTEM_PARTITION_NAME, dataType, true,
                                 CultureInfo.InvariantCulture) == 0)
                    {
                        guid = ImageConstants.SYSTEM_PARTITION_ID;
                    }
                    else
                    {
                        if (string.Compare(ImageConstants.MMOS_PARTITION_NAME, dataType, true,
                                CultureInfo.InvariantCulture) != 0)
                        {
                            throw new ImageStorageException(string.Format(
                                                        "{0}: The partition name {1} is not currently supported.",
                                                        MethodBase.GetCurrentMethod().Name, dataType));
                        }

                        guid = ImageConstants.MMOS_PARTITION_ID;
                    }
                }

                return guid;
            }
        }
    }
}