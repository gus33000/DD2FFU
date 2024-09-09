// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementDataType
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using Microsoft.Win32;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementDataType
    {
        public BcdElementDataType()
        {
        }


        public BcdElementDataType(uint dataType)
        {
            RawValue = dataType;
        }


        public BcdElementDataType(ElementClass elementClass, ElementFormat elementFormat, uint elementSubClass)
        {
            Class = elementClass;
            Format = elementFormat;
            SubClass = elementSubClass;
        }

        public uint RawValue
        {
            get; set;
        }


        public ElementClass Class
        {
            get => (ElementClass)((RawValue & 4026531840U) >> 28);
            set => RawValue = (uint)(((int)RawValue & 268435455) | ((int)value << 28));
        }


        public ElementFormat Format
        {
            get => (ElementFormat)((RawValue & 251658240U) >> 24);
            set => RawValue = (uint)(((int)RawValue & -251658241) | ((int)value << 24));
        }


        public uint SubClass
        {
            get => RawValue & 16777215U;
            set => RawValue = (uint)(((int)RawValue & -16777216) | ((int)value & 16777215));
        }

        public RegistryValueKind RegistryValueType => Format switch
        {
            ElementFormat.Device => RegistryValueKind.Binary,
            ElementFormat.String => RegistryValueKind.String,
            ElementFormat.Object => RegistryValueKind.String,
            ElementFormat.ObjectList => RegistryValueKind.MultiString,
            ElementFormat.Integer => RegistryValueKind.Binary,
            ElementFormat.Boolean => RegistryValueKind.Binary,
            ElementFormat.IntegerList => RegistryValueKind.Binary,
            _ => RegistryValueKind.Binary,
        };

        public override bool Equals(object obj)
        {
            return obj is BcdElementDataType bcdElementDataType && bcdElementDataType.Format == Format &&
                   bcdElementDataType.Class == Class && (int)bcdElementDataType.SubClass == (int)SubClass;
        }

        public override int GetHashCode()
        {
            return (int)(Format ^ (ElementFormat)Class ^ (ElementFormat)SubClass);
        }


        public void LogInfo(IULogger logger, int indentLevel)
        {
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Class:              {0}", Class);
            logger.LogInfo(str + "Format:             {0}", Format);
            if (BcdElementDataTypes.ApplicationTypes.ContainsKey(this))
            {
                logger.LogInfo(str + "SubClass:           {0} (0x{1:x})",
                                BcdElementDataTypes.ApplicationTypes[this], SubClass);
            }
            else if (BcdElementDataTypes.LibraryTypes.ContainsKey(this))
            {
                logger.LogInfo(str + "SubClass:           {0} (0x{1:x})",
                                BcdElementDataTypes.LibraryTypes[this], SubClass);
            }
            else if (BcdElementDataTypes.DeviceTypes.ContainsKey(this))
            {
                logger.LogInfo(str + "SubClass:           {0} (0x{1:x})",
                                BcdElementDataTypes.DeviceTypes[this], SubClass);
            }
            else
            {
                logger.LogInfo(str + "SubClass:           0x{0:x}", SubClass);
            }

            logger.LogInfo(str + "Registry Data Type: {0}", RegistryValueType);
        }

        public override string ToString()
        {
            return string.Format("{0:x8}", RawValue);
        }
    }
}