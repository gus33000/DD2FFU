// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementDataTypeInput
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementDataTypeInput
    {
        [XmlChoiceIdentifier("TypeIdentifier")]
        [XmlElement("WellKnownType", typeof(string))]
        [XmlElement("RawType", typeof(string))]
        public object DataType
        {
            get; set;
        }

        [XmlIgnore]
        public DataTypeChoice TypeIdentifier
        {
            get; set;
        }

        [XmlIgnore]
        public BcdElementDataType Type
        {
            get
            {
                if (TypeIdentifier != DataTypeChoice.WellKnownType)
                {
                    throw new ImageStorageException(string.Format("{0}: Only WellKnownTypes are currently supported.",
                                        MethodBase.GetCurrentMethod().Name));
                }

                BcdElementDataType wellKnownDataType = BcdElementDataTypes.GetWellKnownDataType(DataType as string);
                return wellKnownDataType ?? throw new ImageStorageException(string.Format(
                    "{0}: The element for well known type '{1}' cannot be translated.",
                    MethodBase.GetCurrentMethod().Name, DataType as string));
            }
        }

        public void SaveAsRegFile(TextWriter writer, string path)
        {
            writer.WriteLine("[{0}\\{1:x8}]", path, Type.RawValue);
        }

        public void SaveAsRegData(BcdRegData bcdRegData, string path)
        {
            bcdRegData.AddRegKey(string.Format("{0}\\{1:x8}", path, Type.RawValue));
        }
    }
}