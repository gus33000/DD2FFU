// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdInput
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    [XmlRoot(ElementName = "BootConfigurationDatabase", IsNullable = false,
        Namespace = "http://schemas.microsoft.com/phone/2011/10/BootConfiguration")]
    public class BcdInput
    {
        private BcdInput()
        {
            SaveKeyToRegistry = true;
        }

        [XmlAttribute] public bool SaveKeyToRegistry { get; set; }

        [XmlAttribute] public bool IncludeDescriptions { get; set; }

        [XmlAttribute] public bool IncludeRegistryHeader { get; set; }

        public BcdObjectsInput Objects { get; set; }

        public void SaveAsRegFile(StreamWriter writer, string path)
        {
            Objects.SaveAsRegFile(writer, path);
        }

        public void SaveAsRegData(BcdRegData bcdRegData, string path)
        {
            Objects.SaveAsRegData(bcdRegData, path);
        }
    }
}