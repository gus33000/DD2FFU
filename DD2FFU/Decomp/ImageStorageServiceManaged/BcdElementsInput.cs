// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementsInput
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    [XmlType("Elements")]
    public class BcdElementsInput
    {
        private BcdElementsInput()
        {
            SaveKeyToRegistry = true;
        }

        [XmlElement("Element")]
        public BcdElementInput[] Elements
        {
            get; set;
        }

        [XmlAttribute]
        public bool SaveKeyToRegistry
        {
            get; set;
        }

        public void SaveAsRegFile(StreamWriter writer, string path)
        {
            if (SaveKeyToRegistry)
            {
                writer.WriteLine("[{0}\\Elements]", path);
                writer.WriteLine();
            }

            foreach (BcdElementInput element in Elements)
            {
                element.SaveAsRegFile(writer, path + "\\Elements");
            }
        }

        public void SaveAsRegData(BcdRegData bcdRegData, string path)
        {
            if (SaveKeyToRegistry)
            {
                bcdRegData.AddRegKey(path);
            }

            string path1 = string.Format("{0}\\Elements", path);
            foreach (BcdElementInput element in Elements)
            {
                element.SaveAsRegData(bcdRegData, path1);
            }
        }
    }
}