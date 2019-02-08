// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdElementValueTypeInput
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdElementValueTypeInput
    {
        [XmlChoiceIdentifier("ValueIdentifier")]
        [XmlElement("StringValue", typeof(string))]
        [XmlElement("BooleanValue", typeof(bool))]
        [XmlElement("ObjectValue", typeof(string))]
        [XmlElement("ObjectListValue", typeof(BcdElementObjectListInput))]
        [XmlElement("IntegerValue", typeof(string))]
        [XmlElement("IntegerListValue", typeof(BcdElementIntegerListInput))]
        [XmlElement("DeviceValue", typeof(BcdElementDeviceInput))]
        public object ValueType { get; set; }

        [XmlIgnore] public ValueTypeChoice ValueIdentifier { get; set; }

        private static bool StringToUlong(string valueAsString, out ulong value)
        {
            var flag = true;
            var startIndex = 0;
            if (valueAsString.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                startIndex = 2;
            if (!ulong.TryParse(valueAsString.Substring(startIndex, valueAsString.Length - startIndex),
                NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value))
                flag = false;
            return flag;
        }

        public static void WriteIntegerValue(StreamWriter writer, string elementName, string valueAsString)
        {
            ulong num = 0;
            if (!StringToUlong(valueAsString, out num))
                throw new ImageStorageException(string.Format("{0}: Unable to parse value for element '{1}'.",
                    MethodBase.GetCurrentMethod().Name, elementName));
            var buffer = new byte[8];
            var memoryStream = (MemoryStream) null;
            var binaryWriter = (BinaryWriter) null;
            try
            {
                memoryStream = new MemoryStream(buffer);
                binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.Write(num);
                for (var index = 0; index < buffer.Length; ++index)
                    writer.Write("{0:x2}{1}", buffer[index], index < buffer.Length - 1 ? "," : "");
            }
            finally
            {
                binaryWriter?.Flush();
                if (memoryStream != null)
                {
                    memoryStream.Flush();
                    memoryStream.Close();
                }
            }
        }

        public void WriteIntegerValue(BcdRegData bcdRegData, string path, string valueAsString)
        {
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            WriteIntegerValue(writer, "", valueAsString);
            writer.Flush();
            memoryStream.Position = 0L;
            var end = new StreamReader(memoryStream).ReadToEnd();
            bcdRegData.AddRegValue(path, "Element", end, "REG_BINARY");
        }

        public static void WriteByteArray(TextWriter writer, string elementName, string elementHeader, byte[] value)
        {
            writer.Write(elementHeader);
            var num1 = elementHeader.Length;
            var num2 = 0;
            while (num2 < value.Length - 1)
            {
                while (num1 < 80 && num2 < value.Length - 1)
                {
                    writer.Write("{0:x2},", value[num2++]);
                    num1 += 3;
                }

                if (num1 >= 80)
                {
                    if (num2 < value.Length - 1)
                        writer.WriteLine("\\");
                    num1 = 0;
                }
            }

            var textWriter = writer;
            var format = "{0:x2}";
            var numArray = value;
            // ISSUE: variable of a boxed type
            var local = numArray[numArray.Length - 1];
            textWriter.WriteLine(format, local);
        }

        public static void WriteByteArray(BcdRegData bcdRegData, string path, byte[] value)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            WriteByteArray(streamWriter, "", "", value);
            streamWriter.Flush();
            memoryStream.Position = 0L;
            var end = new StreamReader(memoryStream).ReadToEnd();
            bcdRegData.AddRegValue(path, "Element", end, "REG_BINARY");
        }

        public static void WriteObjectsValue(TextWriter writer, string elementName, string elementHeader,
            string objectsAsStrings)
        {
            var bytes = Encoding.Unicode.GetBytes(objectsAsStrings);
            WriteByteArray(writer, elementName, elementHeader, bytes);
        }

        public void SaveAsRegData(BcdRegData bcdRegData, string path)
        {
            switch (ValueIdentifier)
            {
                case ValueTypeChoice.StringValue:
                    var str1 = new StringBuilder(ValueType as string).ToString();
                    bcdRegData.AddRegValue(path, "Element", str1, "REG_SZ");
                    break;
                case ValueTypeChoice.BooleanValue:
                    bcdRegData.AddRegValue(path, "Element", string.Format("{0}", (bool) ValueType ? "01" : "00"),
                        "REG_BINARY");
                    break;
                case ValueTypeChoice.ObjectValue:
                    var str2 = string.Format("{{{0}}}", BcdObjects.IdFromName(ValueType as string).ToString());
                    bcdRegData.AddRegValue(path, "Element", str2, "REG_SZ");
                    break;
                case ValueTypeChoice.ObjectListValue:
                    (ValueType as BcdElementObjectListInput).SaveAsRegData(bcdRegData, path);
                    break;
                case ValueTypeChoice.IntegerValue:
                    WriteIntegerValue(bcdRegData, path, ValueType as string);
                    break;
                case ValueTypeChoice.IntegerListValue:
                    (ValueType as BcdElementIntegerListInput).SaveAsRegData(bcdRegData, path);
                    break;
                case ValueTypeChoice.DeviceValue:
                    (ValueType as BcdElementDeviceInput).SaveAsRegData(bcdRegData, path);
                    break;
                default:
                    throw new ImageStorageException(string.Format("{0}: Invalid value type for element '{1}'.",
                        MethodBase.GetCurrentMethod().Name, path));
            }
        }

        public void SaveAsRegFile(StreamWriter writer, string elementName)
        {
            switch (ValueIdentifier)
            {
                case ValueTypeChoice.StringValue:
                    var stringBuilder = new StringBuilder(ValueType as string);
                    for (var index = 0; index < stringBuilder.Length; ++index)
                        if (stringBuilder[index] == '\\')
                            stringBuilder.Insert(index++, '\\');
                    writer.WriteLine("\"Element\"=\"{0}\"", stringBuilder);
                    writer.WriteLine();
                    break;
                case ValueTypeChoice.BooleanValue:
                    writer.WriteLine("\"Element\"=hex:{0}", (bool) ValueType ? "01" : "00");
                    writer.WriteLine();
                    break;
                case ValueTypeChoice.ObjectValue:
                    var str = string.Format("\"Element\"=\"{{{0}}}\"",
                        BcdObjects.IdFromName(ValueType as string).ToString());
                    writer.WriteLine("{0}", str);
                    writer.WriteLine();
                    writer.Flush();
                    break;
                case ValueTypeChoice.ObjectListValue:
                    (ValueType as BcdElementObjectListInput).SaveAsRegFile(writer, elementName);
                    break;
                case ValueTypeChoice.IntegerValue:
                    writer.Write("\"Element\"=hex:");
                    WriteIntegerValue(writer, elementName, ValueType as string);
                    writer.WriteLine();
                    writer.WriteLine();
                    break;
                case ValueTypeChoice.IntegerListValue:
                    (ValueType as BcdElementIntegerListInput).SaveAsRegFile(writer, elementName);
                    break;
                case ValueTypeChoice.DeviceValue:
                    (ValueType as BcdElementDeviceInput).SaveAsRegFile(writer, elementName);
                    break;
                default:
                    throw new ImageStorageException(string.Format("{0}: Invalid value type for element '{1}'.",
                        MethodBase.GetCurrentMethod().Name, elementName));
            }
        }
    }
}