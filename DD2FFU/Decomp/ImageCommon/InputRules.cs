// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.InputRules
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ImageCommon.dll

using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class InputRules
    {
        [XmlArrayItem(ElementName = "IntegerRule", IsNullable = false, Type = typeof(InputIntegerRule))] [XmlArray]
        public InputIntegerRule[] IntegerRules;

        [XmlArrayItem(ElementName = "StringRule", IsNullable = false, Type = typeof(InputStringRule))] [XmlArray]
        public InputStringRule[] StringRules;
    }
}