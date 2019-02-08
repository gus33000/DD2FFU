// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdObjectInput
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdObjectInput
    {
        private BcdObjectInput()
        {
            SaveKeyToRegistry = true;
        }

        public string FriendlyName { get; set; }

        public int RawType { get; set; }

        [XmlElement("Id")] public string IdAsString { get; set; }

        public BcdElementsInput Elements { get; set; }

         [XmlIgnore] public uint ObjectType => BcdObjects.ObjectTypeFromName(FriendlyName);

        [XmlIgnore] public Guid Id => BcdObjects.IdFromName(FriendlyName);

        [XmlAttribute] public bool SaveKeyToRegistry { get; set; }

        public void SaveAsRegFile(StreamWriter writer, string path)
        {
            var path1 = string.Format("{0}\\{{{1}}}", path, Id);
            if (SaveKeyToRegistry)
            {
                writer.WriteLine("[{0}]", path1);
                writer.WriteLine();
                writer.WriteLine("[{0}\\Description]", path1);
                writer.WriteLine("\"Type\"=dword:{0:x8}", ObjectType);
                writer.WriteLine();
            }

            Elements.SaveAsRegFile(writer, path1);
        }

        public void SaveAsRegData(BcdRegData bcdRegData, string path)
        {
            var str = string.Format("{0}\\{{{1}}}", path, Id);
            if (SaveKeyToRegistry)
            {
                var regKey = string.Format("{0}\\Description", str);
                bcdRegData.AddRegKey(str);
                bcdRegData.AddRegKey(regKey);
                bcdRegData.AddRegValue(regKey, "Type", string.Format("{0:x8}", ObjectType), "REG_DWORD");
            }

            Elements.SaveAsRegData(bcdRegData, str);
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(FriendlyName))
                return FriendlyName;
            return base.ToString();
        }
    }
}