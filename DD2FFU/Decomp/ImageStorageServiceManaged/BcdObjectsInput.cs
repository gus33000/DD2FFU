// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdObjectsInput
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    [XmlType("Objects")]
    public class BcdObjectsInput
    {
        private BcdObjectsInput()
        {
            SaveKeyToRegistry = true;
        }

        [XmlElement("Object")] public BcdObjectInput[] Objects { get; set; }

        [XmlAttribute] public bool SaveKeyToRegistry { get; set; }

        public void SaveAsRegFile(StreamWriter writer, string path)
        {
            var path1 = path + "\\Objects";
            if (SaveKeyToRegistry)
            {
                writer.WriteLine("[{0}]", path1);
                writer.WriteLine();
            }

            foreach (var bcdObjectInput in Objects)
                bcdObjectInput.SaveAsRegFile(writer, path1);
        }

        public void SaveAsRegData(BcdRegData bcdRegData, string path)
        {
            var str = path + "\\Objects";
            if (SaveKeyToRegistry)
                bcdRegData.AddRegKey(str);
            foreach (var bcdObjectInput in Objects)
                bcdObjectInput.SaveAsRegData(bcdRegData, str);
        }
    }
}