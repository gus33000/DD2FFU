// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.HiveToRegConverter
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public class HiveToRegConverter
    {
        private readonly HashSet<string> m_exclusions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly string m_hiveFile;
        private readonly string m_keyPrefix;
        private TextWriter m_writer;

        public HiveToRegConverter(string hiveFile)
        {
            VerifyHiveFileInput(hiveFile);
            m_hiveFile = hiveFile;
            m_keyPrefix = null;
        }

        public HiveToRegConverter(string hiveFile, string keyPrefix)
        {
            VerifyHiveFileInput(hiveFile);
            m_hiveFile = hiveFile;
            m_keyPrefix = keyPrefix;
        }

        public void VerifyHiveFileInput(string hiveFile)
        {
            if (string.IsNullOrEmpty(hiveFile))
                throw new ArgumentNullException(nameof(hiveFile), "HiveFile cannot be null.");
            if (!LongPathFile.Exists(hiveFile))
                throw new FileNotFoundException(string.Format("Hive file {0} does not exist or cannot be read",
                    hiveFile));
        }

        public void ConvertToReg(string outputFile)
        {
            ConvertToReg(outputFile, null, false);
        }

        public void ConvertToReg(string outputFile, HashSet<string> exclusions)
        {
            ConvertToReg(outputFile, exclusions, false);
        }

        public void ConvertToReg(string outputFile, HashSet<string> exclusions, bool append)
        {
            if (string.IsNullOrEmpty(outputFile))
                throw new ArgumentNullException(nameof(outputFile), "Output file cannot be empty.");
            if (exclusions != null)
                m_exclusions.UnionWith(exclusions);
            var mode = append ? FileMode.Append : FileMode.Create;
            using (m_writer = new StreamWriter(LongPathFile.Open(outputFile, mode, FileAccess.Write), Encoding.Unicode))
            {
                ConvertToStream(!append, null);
            }
        }

        public void ConvertToReg(ref StringBuilder outputStr)
        {
            ConvertToReg(ref outputStr, null);
        }

        public void ConvertToReg(ref StringBuilder outputStr, HashSet<string> exclusions)
        {
            ConvertToReg(ref outputStr, null, true, exclusions);
        }

        public void ConvertToReg(ref StringBuilder outputStr, string subKey, bool outputHeader)
        {
            ConvertToReg(ref outputStr, null, true, null);
        }

        public void ConvertToReg(ref StringBuilder outputStr, string subKey, bool outputHeader,
            HashSet<string> exclusions)
        {
            if (outputStr == null)
                throw new ArgumentNullException(nameof(outputStr));
            if (exclusions != null)
                m_exclusions.UnionWith(exclusions);
            using (m_writer = new StringWriter(outputStr))
            {
                ConvertToStream(outputHeader, subKey);
            }
        }

        private void ConvertToStream(bool outputHeader, string subKey)
        {
            if (outputHeader)
                m_writer.WriteLine("Windows Registry Editor Version 5.00");
            using (var orRegistryKey1 = ORRegistryKey.OpenHive(m_hiveFile, m_keyPrefix))
            {
                var orRegistryKey2 = orRegistryKey1;
                if (!string.IsNullOrEmpty(subKey))
                    orRegistryKey2 = orRegistryKey1.OpenSubKey(subKey);
                WriteKeyContents(orRegistryKey2);
                WalkHive(orRegistryKey2);
            }
        }

        private void WalkHive(ORRegistryKey root)
        {
            foreach (var subkeyname in root.SubKeys.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                using (var orRegistryKey = root.OpenSubKey(subkeyname))
                {
                    try
                    {
                        var num = m_exclusions.Contains(orRegistryKey.FullName + "\\*") ? 1 : 0;
                        var flag = m_exclusions.Contains(orRegistryKey.FullName);
                        if (num == 0)
                        {
                            if (!flag)
                                WriteKeyContents(orRegistryKey);
                            WalkHive(orRegistryKey);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new IUException("Failed to iterate through hive", ex);
                    }
                }
        }

        private void WriteKeyName(string keyname)
        {
            m_writer.WriteLine();
            m_writer.WriteLine("[{0}]", keyname);
        }

        private string FormatValueName(string valueName)
        {
            var stringBuilder1 = new StringBuilder();
            if (valueName.Equals(""))
            {
                stringBuilder1.Append("@=");
            }
            else
            {
                var stringBuilder2 = new StringBuilder(valueName);
                stringBuilder2.Replace("\\", "\\\\").Replace("\"", "\\\"");
                stringBuilder1.AppendFormat("\"{0}\"=", stringBuilder2);
            }

            return stringBuilder1.ToString();
        }

        private string FormatValue(ORRegistryKey key, string valueName)
        {
            var valueKind = key.GetValueKind(valueName);
            var stringBuilder1 = new StringBuilder();
            switch (valueKind)
            {
                case RegistryValueType.String:
                    var stringBuilder2 = new StringBuilder();
                    stringBuilder2.Append(key.GetStringValue(valueName));
                    stringBuilder2.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    stringBuilder1.AppendFormat(CultureInfo.InvariantCulture, "\"{0}\"", new object[1]
                    {
                        stringBuilder2.ToString()
                    });
                    break;
                case RegistryValueType.DWord:
                    var dwordValue = key.GetDwordValue(valueName);
                    stringBuilder1.AppendFormat(CultureInfo.InvariantCulture, "dword:{0:X8}", new object[1]
                    {
                        dwordValue
                    });
                    break;
                case RegistryValueType.MultiString:
                    var byteValue1 = key.GetByteValue(valueName);
                    stringBuilder1.AppendFormat(CultureInfo.InvariantCulture, "hex(7):{0}", new object[1]
                    {
                        OfflineRegUtils.ConvertByteArrayToRegStrings(byteValue1)
                    });
                    var multiStringValue = key.GetMultiStringValue(valueName);
                    stringBuilder1.AppendLine();
                    stringBuilder1.AppendLine(GetMultiStringValuesAsComments(multiStringValue));
                    break;
                default:
                    var byteValue2 = key.GetByteValue(valueName);
                    stringBuilder1.AppendFormat(CultureInfo.InvariantCulture, "hex({0,1:X}):{1}", new object[2]
                    {
                        valueKind,
                        OfflineRegUtils.ConvertByteArrayToRegStrings(byteValue2)
                    });
                    if (valueKind == RegistryValueType.ExpandString)
                    {
                        stringBuilder1.AppendLine();
                        stringBuilder1.AppendLine(GetExpandStringValueAsComments(key.GetStringValue(valueName)));
                    }

                    break;
            }

            return stringBuilder1.ToString();
        }

        private string GetMultiStringValuesAsComments(string[] values)
        {
            var stringBuilder = new StringBuilder(500);
            var num1 = 80;
            if (values != null && values.Length != 0)
            {
                stringBuilder.Append(";Values=");
                var num2 = stringBuilder.Length;
                foreach (var str in values)
                {
                    stringBuilder.AppendFormat("{0},", str);
                    num2 += str.Length + 1;
                    if (num2 > num1)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.Append(";");
                        num2 = 1;
                    }
                }

                stringBuilder.Replace(",", string.Empty, stringBuilder.Length - 1, 1);
            }

            return stringBuilder.ToString();
        }

        private string GetExpandStringValueAsComments(string value)
        {
            return string.Format(";Value={0}", value);
        }

        private void WriteKeyContents(ORRegistryKey key)
        {
            WriteKeyName(key.FullName);
            var str = key.Class;
            if (!string.IsNullOrEmpty(str))
                m_writer.WriteLine(";Class=\"{0}\"", str);
            foreach (var valueName in key.ValueNames.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(FormatValueName(valueName));
                stringBuilder.Append(FormatValue(key, valueName));
                m_writer.WriteLine(stringBuilder.ToString());
            }
        }
    }
}