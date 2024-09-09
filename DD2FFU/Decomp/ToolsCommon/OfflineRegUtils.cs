// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.OfflineRegUtils
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public class OfflineRegUtils
    {
        private static readonly char[] BSLASH_DELIMITER = new char[1]
        {
            '\\'
        };

        public static nint CreateHive()
        {
            nint zero = nint.Zero;
            int hive = OffRegNativeMethods.ORCreateHive(ref zero);
            return hive != 0 ? throw new Win32Exception(hive) : zero;
        }

        public static nint CreateKey(nint handle, string keyName)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            if (string.IsNullOrEmpty(nameof(keyName)))
            {
                throw new ArgumentNullException(nameof(keyName));
            }

            nint zero = nint.Zero;
            uint dwDisposition = 0;
            foreach (string str in keyName.Split(BSLASH_DELIMITER))
            {
                int key = OffRegNativeMethods.ORCreateKey(handle, keyName, null, 0U, null, ref zero, ref dwDisposition);
                if (key != 0)
                {
                    throw new Win32Exception(key);
                }
            }

            return zero;
        }

        public static void SetValue(nint handle, string valueName, RegistryValueType type, byte[] value)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            valueName ??= string.Empty;
            nint Handle = handle;
            string lpValueName = valueName;
            int num = (int)type;
            byte[] pvData = value;
            int length = pvData.Length;
            int error = OffRegNativeMethods.ORSetValue(Handle, lpValueName, (uint)num, pvData, (uint)length);
            if (error != 0)
            {
                throw new Win32Exception(error);
            }
        }

        public static void DeleteValue(nint handle, string valueName)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            valueName ??= string.Empty;
            int error = OffRegNativeMethods.ORDeleteValue(handle, valueName);
            if (error != 0)
            {
                throw new Win32Exception(error);
            }
        }

        public static void DeleteKey(nint handle, string keyName)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            int error = OffRegNativeMethods.ORDeleteKey(handle, keyName);
            if (error != 0)
            {
                throw new Win32Exception(error);
            }
        }

        public static nint OpenHive(string hivefile)
        {
            if (string.IsNullOrEmpty(hivefile))
            {
                throw new ArgumentNullException(nameof(hivefile));
            }

            nint zero = nint.Zero;
            int num = 10;
            int error = 0;
            while (num > 0)
            {
                error = OffRegNativeMethods.OROpenHive(hivefile, ref zero);
                if (error != 0)
                {
                    --num;
                    Thread.Sleep(2000);
                }
                else
                {
                    num = 0;
                }
            }

            return error != 0 ? throw new Win32Exception(error) : zero;
        }

        public static void SaveHive(nint handle, string path, int osMajor, int osMinor)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (File.Exists(path))
            {
                FileUtils.DeleteFile(path);
            }

            int error = OffRegNativeMethods.ORSaveHive(handle, path, osMajor, osMinor);
            if (error != 0)
            {
                throw new Win32Exception(error);
            }
        }

        public static void CloseHive(nint handle)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            int error = OffRegNativeMethods.ORCloseHive(handle);
            if (error != 0)
            {
                throw new Win32Exception(error);
            }
        }

        public static nint OpenKey(nint handle, string subKeyName)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            if (string.IsNullOrEmpty(nameof(subKeyName)))
            {
                throw new ArgumentNullException(nameof(subKeyName));
            }

            nint zero = nint.Zero;
            int error = OffRegNativeMethods.OROpenKey(handle, subKeyName, ref zero);
            return error != 0 ? throw new Win32Exception(error) : zero;
        }

        public static void CloseKey(nint handle)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            int error = OffRegNativeMethods.ORCloseKey(handle);
            if (error != 0)
            {
                throw new Win32Exception(error);
            }
        }

        public static void ConvertHiveToReg(string inputHiveFile, string outputRegFile)
        {
            new HiveToRegConverter(inputHiveFile).ConvertToReg(outputRegFile);
        }

        public static void ConvertHiveToReg(string inputHiveFile, string outputRegFile, string keyPrefix)
        {
            new HiveToRegConverter(inputHiveFile, keyPrefix).ConvertToReg(outputRegFile);
        }

        public static void ConvertHiveToReg(string inputHiveFile, string outputRegFile, string keyPrefix,
            bool appendExisting)
        {
            new HiveToRegConverter(inputHiveFile, keyPrefix).ConvertToReg(outputRegFile, null, appendExisting);
        }

        public static string ConvertByteArrayToRegStrings(byte[] data)
        {
            return ConvertByteArrayToRegStrings(data, 40);
        }

        public static string ConvertByteArrayToRegStrings(byte[] data, int maxOnALine)
        {
            string str1;
            if (-1 == maxOnALine)
            {
                str1 = BitConverter.ToString(data).Replace('-', ',');
            }
            else
            {
                int startIndex = 0;
                int length1 = data.Length;
                StringBuilder stringBuilder = new();
                while (length1 > 0)
                {
                    int length2 = length1 > maxOnALine ? maxOnALine : length1;
                    string str2 = BitConverter.ToString(data, startIndex, length2);
                    startIndex += length2;
                    length1 -= length2;
                    string str3 = str2.Replace('-', ',');
                    _ = stringBuilder.Append(str3);
                    if (length1 > 0)
                    {
                        _ = stringBuilder.Append(",\\");
                        _ = stringBuilder.AppendLine();
                    }
                }

                str1 = stringBuilder.ToString();
            }

            return str1;
        }

        public static RegistryValueType GetValueType(nint handle, string valueName)
        {
            uint pcbData = 0;
            int error = OffRegNativeMethods.ORGetValue(handle, null, valueName, out uint pdwType, null, ref pcbData);
            return error != 0 ? throw new Win32Exception(error) : (RegistryValueType)pdwType;
        }

        public static List<KeyValuePair<string, RegistryValueType>> GetValueNamesAndTypes(nint handle)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            uint index = 0;
            StringBuilder lpValueName = new(1024);
            List<KeyValuePair<string, RegistryValueType>> keyValuePairList = [];
            int error;
            do
            {
                uint capacity = (uint)lpValueName.Capacity;
                error = OffRegNativeMethods.OREnumValue(handle, index, lpValueName, ref capacity, out uint lpType,
                    nint.Zero, nint.Zero);
                if (error != 0)
                {
                    if (error != 259)
                    {
                        throw new Win32Exception(error);
                    }
                }
                else
                {
                    string key = lpValueName.ToString();
                    RegistryValueType registryValueType = (RegistryValueType)lpType;
                    keyValuePairList.Add(new KeyValuePair<string, RegistryValueType>(key, registryValueType));
                    ++index;
                }
            } while (error != 259);

            return keyValuePairList;
        }

        public static string[] GetValueNames(nint handle)
        {
            return handle == nint.Zero
                ? throw new ArgumentNullException(nameof(handle), "Handle cannot be empty.")
                : GetValueNamesAndTypes(handle).Select(a => a.Key).ToArray();
        }

        public static string GetClass(nint handle)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle), "Handle cannot be empty.");
            }

            StringBuilder classname = new(128);
            uint capacity = (uint)classname.Capacity;
            uint[] numArray = new uint[8];
            nint zero = nint.Zero;
            int error = OffRegNativeMethods.ORQueryInfoKey(handle, classname, ref capacity, out numArray[0],
                out numArray[1], out numArray[3], out numArray[4], out numArray[5], out numArray[6], out numArray[7],
                zero);
            if (error == 234)
            {
                ++capacity;
                classname.Capacity = (int)capacity;
                error = OffRegNativeMethods.ORQueryInfoKey(handle, classname, ref capacity, out numArray[0],
                    out numArray[1], out numArray[3], out numArray[4], out numArray[5], out numArray[6],
                    out numArray[7], zero);
            }

            return error != 0 ? throw new Win32Exception(error) : classname.ToString();
        }

        public static byte[] GetValue(nint handle, string valueName)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle), "Handle cannot be empty.");
            }

            uint pcbData = 0;

            int error1 = OffRegNativeMethods.ORGetValue(handle, null, valueName, out _, null, ref pcbData);
            if (error1 != 0)
            {
                throw new Win32Exception(error1);
            }

            byte[] pvData = new byte[(int)pcbData];
            int error2 = OffRegNativeMethods.ORGetValue(handle, null, valueName, out _, pvData, ref pcbData);
            return error2 != 0 ? throw new Win32Exception(error2) : pvData;
        }

        public static string[] GetSubKeys(nint registryKey)
        {
            if (registryKey == nint.Zero)
            {
                throw new ArgumentNullException(nameof(registryKey), "registryKey pointer cannot be empty.");
            }

            uint dwIndex = 0;
            StringBuilder name = new(1024);
            List<string> stringList = [];
            int error;
            do
            {
                uint classnamecount = 0;
                nint zero = nint.Zero;
                uint capacity = (uint)name.Capacity;
                error = OffRegNativeMethods.OREnumKey(registryKey, dwIndex, name, ref capacity, null,
                    ref classnamecount, ref zero);
                if (error != 0)
                {
                    if (error != 259)
                    {
                        throw new Win32Exception(error);
                    }
                }
                else
                {
                    stringList.Add(name.ToString());
                    ++dwIndex;
                }
            } while (error != 259);

            return stringList.ToArray();
        }

        public static byte[] GetRawRegistrySecurity(nint handle)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            uint size = 0;
            int num1 = 234;
            int keySecurity1 = OffRegNativeMethods.ORGetKeySecurity(handle,
                SecurityInformationFlags.DACL_SECURITY_INFORMATION |
                SecurityInformationFlags.SACL_SECURITY_INFORMATION | SecurityInformationFlags.MANDATORY_ACCESS_LABEL,
                null, ref size);
            int num2 = keySecurity1;
            if (num1 != num2)
            {
                throw new Win32Exception(keySecurity1);
            }

            byte[] lpSecBuf = new byte[(int)size];
            int keySecurity2 = OffRegNativeMethods.ORGetKeySecurity(handle,
                SecurityInformationFlags.DACL_SECURITY_INFORMATION |
                SecurityInformationFlags.SACL_SECURITY_INFORMATION | SecurityInformationFlags.MANDATORY_ACCESS_LABEL,
                lpSecBuf, ref size);
            return keySecurity2 != 0 ? throw new Win32Exception(keySecurity2) : lpSecBuf;
        }

        public static void SetRawRegistrySecurity(nint handle, byte[] buf)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            int error = OffRegNativeMethods.ORSetKeySecurity(handle,
                            SecurityInformationFlags.DACL_SECURITY_INFORMATION |
                            SecurityInformationFlags.SACL_SECURITY_INFORMATION | SecurityInformationFlags.MANDATORY_ACCESS_LABEL,
                            buf);
            if (error != 0)
            {
                throw new Win32Exception(error);
            }
        }

        public static RegistrySecurity GetRegistrySecurity(nint handle)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            byte[] registrySecurity1 = GetRawRegistrySecurity(handle);
            _ = SecurityUtils.ConvertSDToStringSD(registrySecurity1,
                SecurityInformationFlags.SACL_SECURITY_INFORMATION | SecurityInformationFlags.MANDATORY_ACCESS_LABEL);
            RegistrySecurity registrySecurity2 = new();
            registrySecurity2.SetSecurityDescriptorBinaryForm(registrySecurity1);
            return registrySecurity2;
        }

        public static int GetVirtualFlags(nint handle)
        {
            if (handle == nint.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            int pbFlags = 0;
            int virtualFlags = OffRegNativeMethods.ORGetVirtualFlags(handle, ref pbFlags);
            return virtualFlags != 0 ? throw new Win32Exception(virtualFlags) : pbFlags;
        }

        public static int ExtractFromHive(string hivePath, RegistryValueType type, string targetPath)
        {
            if (string.IsNullOrEmpty(nameof(hivePath)))
            {
                throw new ArgumentNullException(nameof(hivePath));
            }

            if (string.IsNullOrEmpty(nameof(targetPath)))
            {
                throw new ArgumentNullException(nameof(targetPath));
            }

            if (!File.Exists(hivePath))
            {
                throw new FileNotFoundException("Hive file {0} does not exist", hivePath);
            }

            int num = 0;
            bool flag = false;
            using (ORRegistryKey srcHiveRoot = ORRegistryKey.OpenHive(hivePath))
            {
                using (ORRegistryKey emptyHive = ORRegistryKey.CreateEmptyHive())
                {
                    flag = 0 < (num = ExtractFromHiveRecursive(srcHiveRoot, type, emptyHive));
                    if (flag)
                    {
                        emptyHive.SaveHive(targetPath);
                    }
                }

                if (flag)
                {
                    srcHiveRoot.SaveHive(hivePath);
                }
            }

            return num;
        }

        private static int ExtractFromHiveRecursive(ORRegistryKey srcHiveRoot, RegistryValueType type,
            ORRegistryKey dstHiveRoot)
        {
            int num = 0;
            string fullName = srcHiveRoot.FullName;
            foreach (string str in srcHiveRoot.ValueNameAndTypes.Where(p => p.Value == RegistryValueType.MultiString)
                .Select(q => q.Key))
            {
                string valueName = string.IsNullOrEmpty(str) ? null : str;
                string[] multiStringValue = srcHiveRoot.GetMultiStringValue(valueName);
                using (ORRegistryKey subKey = dstHiveRoot.CreateSubKey(fullName))
                {
                    subKey.SetValue(valueName, multiStringValue);
                    ++num;
                }

                srcHiveRoot.DeleteValue(valueName);
            }

            foreach (string subKey in srcHiveRoot.SubKeys)
            {
                using ORRegistryKey srcHiveRoot1 = srcHiveRoot.OpenSubKey(subKey);
                num += ExtractFromHiveRecursive(srcHiveRoot1, type, dstHiveRoot);
            }

            return num;
        }
    }
}