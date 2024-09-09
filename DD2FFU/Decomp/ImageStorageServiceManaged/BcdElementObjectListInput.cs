﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementObjectListInput
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementObjectListInput
    {
        [XmlArrayItem(ElementName = "StringValue", IsNullable = false, Type = typeof(string))]
        [XmlArray]
        public string[] StringValues
        {
            get; set;
        }

        public void SaveAsRegFile(TextWriter writer, string elementName)
        {
            StringBuilder stringBuilder = new();
            for (int index = 0; index < StringValues.Length; ++index)
            {
                System.Guid guid = BcdObjects.IdFromName(StringValues[index]);
                _ = stringBuilder.Append(string.Format("{{{0}}}", guid));
                _ = stringBuilder.Append("\0");
            }

            _ = stringBuilder.Append("\0");
            BcdElementValueTypeInput.WriteObjectsValue(writer, elementName, "\"Element\"=hex(7):",
                stringBuilder.ToString());
            foreach (string stringValue in StringValues)
            {
                System.Guid guid = BcdObjects.IdFromName(stringValue);
                writer.WriteLine(";Values={{{0}}}, \"{1}\"", guid, stringValue);
            }

            writer.WriteLine();
        }

        public void SaveAsRegData(BcdRegData bcdRegData, string path)
        {
            string str1 = null;
            foreach (string stringValue in StringValues)
            {
                str1 = str1 + "\"{" + BcdObjects.IdFromName(stringValue) + "}\",";
            }

            string str2 = str1.TrimEnd(',');
            bcdRegData.AddRegValue(path, "Element", str2, "REG_MULTI_SZ");
        }
    }
}