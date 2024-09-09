// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementIntegerListInput
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementIntegerListInput
    {
        [XmlArrayItem(ElementName = "StringValue", IsNullable = false, Type = typeof(string))]
        [XmlArray]
        public string[] StringValues
        {
            get; set;
        }

        public void SaveAsRegFile(StreamWriter writer, string elementName)
        {
            writer.Write("\"Element\"=hex:");
            for (int index = 0; index < StringValues.Length; ++index)
            {
                BcdElementValueTypeInput.WriteIntegerValue(writer, elementName, StringValues[index]);
                if (index < StringValues.Length - 1)
                {
                    writer.Write(",");
                }
            }

            writer.WriteLine();
            writer.WriteLine();
        }

        public void SaveAsRegData(BcdRegData bcdRegData, string path)
        {
            MemoryStream memoryStream = new();
            StreamWriter writer = new(memoryStream);
            for (int index = 0; index < StringValues.Length; ++index)
            {
                BcdElementValueTypeInput.WriteIntegerValue(writer, "", StringValues[index]);
            }

            writer.Flush();
            memoryStream.Position = 0L;
            string end = new StreamReader(memoryStream).ReadToEnd();
            bcdRegData.AddRegValue(path, "Element", end, "REG_BINARY");
        }
    }
}