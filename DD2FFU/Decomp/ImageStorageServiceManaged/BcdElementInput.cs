// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementInput
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Reflection;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementInput
    {
        public BcdElementDataTypeInput DataType { get; set; }

        public BcdElementValueTypeInput ValueType { get; set; }

        protected void RegFilePreProcessing()
        {
            if (DataType.TypeIdentifier != DataTypeChoice.WellKnownType ||
                BcdElementDataTypes.GetWellKnownDataType(DataType.DataType as string) !=
                BcdElementDataTypes.CustomActionsList)
                return;
            if (ValueType.ValueIdentifier != ValueTypeChoice.IntegerListValue)
                throw new ImageStorageException(string.Format(
                    "{0}: A custom action list should have an integer list associated with it.",
                    MethodBase.GetCurrentMethod().Name));
            var valueType = ValueType.ValueType as BcdElementIntegerListInput;
            if (valueType.StringValues.Length % 2 != 0)
                throw new ImageStorageException(string.Format(
                    "{0}: A custom action list should have one element associated with each scan key code.",
                    MethodBase.GetCurrentMethod().Name));
            var num = 0;
            while (num < valueType.StringValues.Length)
            {
                var wellKnownDataType = BcdElementDataTypes.GetWellKnownDataType(valueType.StringValues[num + 1]);
                if (wellKnownDataType != null)
                    valueType.StringValues[num + 1] = string.Format("{0:x8}", wellKnownDataType.RawValue);
                num += 2;
            }
        }

        public void SaveAsRegFile(StreamWriter writer, string path)
        {
            RegFilePreProcessing();
            DataType.SaveAsRegFile(writer, path);
            ValueType.SaveAsRegFile(writer, string.Format("{0:x8}", DataType.Type));
        }

        public void SaveAsRegData(BcdRegData bcdRegData, string path)
        {
            RegFilePreProcessing();
            DataType.SaveAsRegData(bcdRegData, path);
            ValueType.SaveAsRegData(bcdRegData, string.Format("{0}\\{1:x8}", path, DataType.Type));
        }
    }
}