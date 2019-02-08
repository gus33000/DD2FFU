// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.ORRegistryKey
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public class ORRegistryKey : IDisposable
    {
        private const string STR_ROOT = "\\";
        private const string STR_NULLCHAR = "\0";

        private readonly char[] BSLASH_DELIMITER = new char[1]
        {
            '\\'
        };

        private readonly ConcurrentDictionary<ORRegistryKey, bool> m_children =
            new ConcurrentDictionary<ORRegistryKey, bool>();

        private IntPtr m_handle = IntPtr.Zero;
        private readonly bool m_isRoot;

        private ORRegistryKey(string name, IntPtr handle, bool isRoot, ORRegistryKey parent)
        {
            FullName = name;
            m_handle = handle;
            m_isRoot = isRoot;
            Parent = parent;
            if (Parent == null)
                return;
            Parent.m_children[this] = true;
        }

        public ORRegistryKey Parent { get; }

        public string[] SubKeys => OfflineRegUtils.GetSubKeys(m_handle);

        public string FullName { get; } = string.Empty;

        public string Class => OfflineRegUtils.GetClass(m_handle);

        public string[] ValueNames => OfflineRegUtils.GetValueNames(m_handle);

        public List<KeyValuePair<string, RegistryValueType>> ValueNameAndTypes =>
            OfflineRegUtils.GetValueNamesAndTypes(m_handle);

        public RegistrySecurity RegistrySecurity => OfflineRegUtils.GetRegistrySecurity(m_handle);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ORRegistryKey()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            foreach (var key in m_children.Keys)
                key.Close();
            m_children.Clear();
            if (Parent != null)
            {
                bool flag;
                Parent.m_children.TryRemove(this, out flag);
            }

            Close();
        }

        public static ORRegistryKey OpenHive(string hivefile)
        {
            return OpenHive(hivefile, null);
        }

        public static ORRegistryKey OpenHive(string hivefile, string prefix)
        {
            if (prefix == null)
                prefix = "\\";
            return new ORRegistryKey(prefix, OfflineRegUtils.OpenHive(hivefile), true, null);
        }

        public static ORRegistryKey CreateEmptyHive()
        {
            return CreateEmptyHive(null);
        }

        public static ORRegistryKey CreateEmptyHive(string prefix)
        {
            return new ORRegistryKey(string.IsNullOrEmpty(prefix) ? "\\" : prefix, OfflineRegUtils.CreateHive(), true,
                null);
        }

        public ORRegistryKey OpenSubKey(string subkeyname)
        {
            ORRegistryKey orRegistryKey1;
            if (-1 < subkeyname.IndexOf("\\", StringComparison.OrdinalIgnoreCase))
            {
                var strArray = subkeyname.Split(BSLASH_DELIMITER);
                var orRegistryKey2 = this;
                var orRegistryKey3 = (ORRegistryKey) null;
                foreach (var subkeyname1 in strArray)
                {
                    orRegistryKey3 = orRegistryKey2.OpenSubKey(subkeyname1);
                    orRegistryKey2 = orRegistryKey3;
                }

                orRegistryKey1 = orRegistryKey3;
            }
            else
            {
                var handle = OfflineRegUtils.OpenKey(m_handle, subkeyname);
                orRegistryKey1 = new ORRegistryKey(CombineSubKeys(FullName, subkeyname), handle, false, this);
            }

            return orRegistryKey1;
        }

        public RegistryValueType GetValueKind(string valueName)
        {
            return OfflineRegUtils.GetValueType(m_handle, valueName);
        }

        public byte[] GetByteValue(string valueName)
        {
            return OfflineRegUtils.GetValue(m_handle, valueName);
        }

        
        public uint GetDwordValue(string valueName)
        {
            var byteValue = GetByteValue(valueName);
            if (byteValue.Length != 0)
                return BitConverter.ToUInt32(byteValue, 0);
            return 0;
        }

        
        public ulong GetQwordValue(string valueName)
        {
            var byteValue = GetByteValue(valueName);
            if (byteValue.Length != 0)
                return BitConverter.ToUInt64(byteValue, 0);
            return 0;
        }

        public string GetStringValue(string valueName)
        {
            var byteValue = GetByteValue(valueName);
            var empty = string.Empty;
            string str;
            if (byteValue.Length > 1)
            {
                var numArray1 = byteValue;
                if (numArray1[numArray1.Length - 1] == 0)
                {
                    var numArray2 = byteValue;
                    if (numArray2[numArray2.Length - 2] == 0)
                    {
                        str = Encoding.Unicode.GetString(byteValue, 0, byteValue.Length - 2);
                        goto label_5;
                    }
                }
            }

            str = Encoding.Unicode.GetString(byteValue);
            label_5:
            return str;
        }

        public string[] GetMultiStringValue(string valueName)
        {
            return Encoding.Unicode.GetString(GetByteValue(valueName))
                .Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
        }

        public object GetValue(string valueName)
        {
            var valueKind = GetValueKind(valueName);
            var obj = (object) null;
            switch (valueKind)
            {
                case RegistryValueType.None:
                    obj = GetByteValue(valueName);
                    break;
                case RegistryValueType.String:
                    obj = GetStringValue(valueName);
                    break;
                case RegistryValueType.ExpandString:
                    obj = GetStringValue(valueName);
                    break;
                case RegistryValueType.Binary:
                    obj = GetByteValue(valueName);
                    break;
                case RegistryValueType.DWord:
                    obj = GetDwordValue(valueName);
                    break;
                case RegistryValueType.DWordBigEndian:
                    obj = GetByteValue(valueName);
                    break;
                case RegistryValueType.Link:
                    obj = GetByteValue(valueName);
                    break;
                case RegistryValueType.MultiString:
                    obj = GetMultiStringValue(valueName);
                    break;
                case RegistryValueType.RegResourceList:
                    obj = GetByteValue(valueName);
                    break;
                case RegistryValueType.RegFullResourceDescriptor:
                    obj = GetByteValue(valueName);
                    break;
                case RegistryValueType.RegResourceRequirementsList:
                    obj = GetByteValue(valueName);
                    break;
                case RegistryValueType.QWord:
                    obj = GetQwordValue(valueName);
                    break;
            }

            return obj;
        }

        public void SaveHive(string path)
        {
            if (!m_isRoot)
                throw new IUException("Invalid operation - This registry key does not represent hive root");
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            OfflineRegUtils.SaveHive(m_handle, path, 6, 3);
        }

        public ORRegistryKey CreateSubKey(string subkeyName)
        {
            ORRegistryKey orRegistryKey1;
            if (-1 != subkeyName.IndexOf("\\", StringComparison.OrdinalIgnoreCase))
            {
                var strArray = subkeyName.Split(BSLASH_DELIMITER, StringSplitOptions.RemoveEmptyEntries);
                var orRegistryKey2 = this;
                var orRegistryKey3 = (ORRegistryKey) null;
                foreach (var subkeyName1 in strArray)
                {
                    orRegistryKey3 = orRegistryKey2.CreateSubKey(subkeyName1);
                    orRegistryKey2 = orRegistryKey3;
                }

                orRegistryKey1 = orRegistryKey3;
            }
            else
            {
                var key = OfflineRegUtils.CreateKey(m_handle, subkeyName);
                orRegistryKey1 = new ORRegistryKey(CombineSubKeys(FullName, subkeyName), key, false, this);
            }

            return orRegistryKey1;
        }

        public void SetValue(string valueName, byte[] value)
        {
            OfflineRegUtils.SetValue(m_handle, valueName, RegistryValueType.Binary, value);
        }

        public void SetValue(string valueName, string value)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
            OfflineRegUtils.SetValue(m_handle, valueName, RegistryValueType.String, bytes);
        }

        public void SetValue(string valueName, string[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            var stringBuilder = new StringBuilder(1024);
            foreach (var str in values)
                stringBuilder.AppendFormat("{0}{1}", str, "\0");
            stringBuilder.Append("\0");
            var bytes = Encoding.Unicode.GetBytes(stringBuilder.ToString());
            OfflineRegUtils.SetValue(m_handle, valueName, RegistryValueType.MultiString, bytes);
        }

        public void SetValue(string valueName, int value)
        {
            var bytes = BitConverter.GetBytes(value);
            OfflineRegUtils.SetValue(m_handle, valueName, RegistryValueType.DWord, bytes);
        }

        public void SetValue(string valueName, long value)
        {
            var bytes = BitConverter.GetBytes(value);
            OfflineRegUtils.SetValue(m_handle, valueName, RegistryValueType.QWord, bytes);
        }

        public void DeleteValue(string valueName)
        {
            OfflineRegUtils.DeleteValue(m_handle, valueName);
        }

        public void DeleteKey(string keyName)
        {
            OfflineRegUtils.DeleteKey(m_handle, keyName);
        }

        private string CombineSubKeys(string path1, string path2)
        {
            if (path1 == null)
                throw new ArgumentNullException(nameof(path1),
                    "The first registry key path to combine cannot be null.");
            if (path2 == null)
                throw new ArgumentNullException(nameof(path2),
                    "The second registry key path to combine cannot be null.");
            if (-1 < path2.IndexOf("\\", StringComparison.OrdinalIgnoreCase) || path1.Length == 0)
                return path2;
            if (path2.Length == 0)
                return path1;
            if (path1.Length == path1.LastIndexOfAny(BSLASH_DELIMITER) + 1)
                return path1 + path2;
            return path1 + BSLASH_DELIMITER[0] + path2;
        }

        private void Close()
        {
            if (!(m_handle != IntPtr.Zero))
                return;
            if (m_isRoot)
                OfflineRegUtils.CloseHive(m_handle);
            else
                OfflineRegUtils.CloseKey(m_handle);
            m_handle = IntPtr.Zero;
        }
    }
}