// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdRegValue
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.Globalization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdRegValue
    {
        public BcdRegValue(string name, string value, string type)
        {
            if (!(type == "REG_BINARY"))
            {
                if (type == "REG_DWORD")
                    value = string.Format("0x{0}", value.ToUpper(CultureInfo.InvariantCulture));
            }
            else
            {
                value = TrimBinary(value).ToUpper(CultureInfo.InvariantCulture);
            }

            Name = name;
            Value = value;
            Type = type;
        }

        public string Name { get; }

        public string Value { get; }

        public string Type { get; }

        private string TrimBinary(string regBinaryStr)
        {
            return regBinaryStr.Replace("\r\n", "").Replace(",", "").Replace("\\", "").Trim();
        }
    }
}