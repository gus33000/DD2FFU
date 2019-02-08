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

        public static IntPtr CreateHive()
        {
            var zero = IntPtr.Zero;
            var hive = OffRegNativeMethods.ORCreateHive(ref zero);
            if (hive != 0)
                throw new Win32Exception(hive);
            return zero;
        }

        public static IntPtr CreateKey(IntPtr handle, string keyName)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));
            if (string.IsNullOrEmpty(nameof(keyName)))
                throw new ArgumentNullException(nameof(keyName));
            var zero = IntPtr.Zero;
            uint dwDisposition = 0;
            foreach (var str in keyName.Split(BSLASH_DELIMITER))
            {
                var key = OffRegNativeMethods.ORCreateKey(handle, keyName, null, 0U, null, ref zero, ref dwDisposition);
                if (key != 0)
                    throw new Win32Exception(key);
            }

            return zero;
        }

        public static void SetValue(IntPtr handle, string valueName, RegistryValueType type, byte[] value)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));
            if (valueName == null)
                valueName = string.Empty;
            var Handle = handle;
            var lpValueName = valueName;
            var num = (int) type;
            var pvData = value;
            var length = pvData.Length;
            var error = OffRegNativeMethods.ORSetValue(Handle, lpValueName, (uint) num, pvData, (uint) length);
            if (error != 0)
                throw new Win32Exception(error);
        }

        public static void DeleteValue(IntPtr handle, string valueName)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));
            if (valueName == null)
                valueName = string.Empty;
            var error = OffRegNativeMethods.ORDeleteValue(handle, valueName);
            if (error != 0)
                throw new Win32Exception(error);
        }

        public static void DeleteKey(IntPtr handle, string keyName)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));
            var error = OffRegNativeMethods.ORDeleteKey(handle, keyName);
            if (error != 0)
                throw new Win32Exception(error);
        }

        public static IntPtr OpenHive(string hivefile)
        {
            if (string.IsNullOrEmpty(hivefile))
                throw new ArgumentNullException(nameof(hivefile));
            var zero = IntPtr.Zero;
            var num = 10;
            var error = 0;
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

            if (error != 0)
                throw new Win32Exception(error);
            return zero;
        }

        public static void SaveHive(IntPtr handle, string path, int osMajor, int osMinor)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (File.Exists(path))
                FileUtils.DeleteFile(path);
            var error = OffRegNativeMethods.ORSaveHive(handle, path, osMajor, osMinor);
            if (error != 0)
                throw new Win32Exception(error);
        }

        public static void CloseHive(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));
            var error = OffRegNativeMethods.ORCloseHive(handle);
            if (error != 0)
                throw new Win32Exception(error);
        }

        public static IntPtr OpenKey(IntPtr handle, string subKeyName)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));
            if (string.IsNullOrEmpty(nameof(subKeyName)))
                throw new ArgumentNullException(nameof(subKeyName));
            var zero = IntPtr.Zero;
            var error = OffRegNativeMethods.OROpenKey(handle, subKeyName, ref zero);
            if (error != 0)
                throw new Win32Exception(error);
            return zero;
        }

        public static void CloseKey(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));
            var error = OffRegNativeMethods.ORCloseKey(handle);
            if (error != 0)
                throw new Win32Exception(error);
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
            var empty = string.Empty;
            string str1;
            if (-1 == maxOnALine)
            {
                str1 = BitConverter.ToString(data).Replace('-', ',');
            }
            else
            {
                var startIndex = 0;
                var length1 = data.Length;
                var stringBuilder = new StringBuilder();
                while (length1 > 0)
                {
                    var length2 = length1 > maxOnALine ? maxOnALine : length1;
                    var str2 = BitConverter.ToString(data, startIndex, length2);
                    startIndex += length2;
                    length1 -= length2;
                    var str3 = str2.Replace('-', ',');
                    stringBuilder.Append(str3);
                    if (length1 > 0)
                    {
                        stringBuilder.Append(",\\");
                        stringBuilder.AppendLine();
                    }
                }

                str1 = stringBuilder.ToString();
            }

            return str1;
        }

        public static RegistryValueType GetValueType(IntPtr handle, string valueName)
        {
            uint pdwType = 0;
            uint pcbData = 0;
            var error = OffRegNativeMethods.ORGetValue(handle, null, valueName, out pdwType, null, ref pcbData);
            if (error != 0)
                throw new Win32Exception(error);
            return (RegistryValueType) pdwType;
        }

        public static List<KeyValuePair<string, RegistryValueType>> GetValueNamesAndTypes(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));
            uint index = 0;
            var lpValueName = new StringBuilder(1024);
            var keyValuePairList = new List<KeyValuePair<string, RegistryValueType>>();
            int error;
            do
            {
                var capacity = (uint) lpValueName.Capacity;
                uint lpType = 0;
                error = OffRegNativeMethods.OREnumValue(handle, index, lpValueName, ref capacity, out lpType,
                    IntPtr.Zero, IntPtr.Zero);
                if (error != 0)
                {
                    if (error != 259)
                        throw new Win32Exception(error);
                }
                else
                {
                    var key = lpValueName.ToString();
                    var registryValueType = (RegistryValueType) lpType;
                    keyValuePairList.Add(new KeyValuePair<string, RegistryValueType>(key, registryValueType));
                    ++index;
                }
            } while (error != 259);

            return keyValuePairList;
        }

        public static string[] GetValueNames(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle), "Handle cannot be empty.");
            return GetValueNamesAndTypes(handle).Select(a => a.Key).ToArray();
        }

        public static string GetClass(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle), "Handle cannot be empty.");
            var classname = new StringBuilder(128);
            var capacity = (uint) classname.Capacity;
            var numArray = new uint[8];
            var zero = IntPtr.Zero;
            var error = OffRegNativeMethods.ORQueryInfoKey(handle, classname, ref capacity, out numArray[0],
                out numArray[1], out numArray[3], out numArray[4], out numArray[5], out numArray[6], out numArray[7],
                zero);
            if (error == 234)
            {
                ++capacity;
                classname.Capacity = (int) capacity;
                error = OffRegNativeMethods.ORQueryInfoKey(handle, classname, ref capacity, out numArray[0],
                    out numArray[1], out numArray[3], out numArray[4], out numArray[5], out numArray[6],
                    out numArray[7], zero);
            }

            if (error != 0)
                throw new Win32Exception(error);
            return classname.ToString();
        }

        public static byte[] GetValue(IntPtr handle, string valueName)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle), "Handle cannot be empty.");
            uint pdwType = 0;
            uint pcbData = 0;
            var error1 = OffRegNativeMethods.ORGetValue(handle, null, valueName, out pdwType, null, ref pcbData);
            if (error1 != 0)
                throw new Win32Exception(error1);
            var pvData = new byte[(int) pcbData];
            var error2 = OffRegNativeMethods.ORGetValue(handle, null, valueName, out pdwType, pvData, ref pcbData);
            if (error2 != 0)
                throw new Win32Exception(error2);
            return pvData;
        }

        public static string[] GetSubKeys(IntPtr registryKey)
        {
            if (registryKey == IntPtr.Zero)
                throw new ArgumentNullException(nameof(registryKey), "registryKey pointer cannot be empty.");
            uint dwIndex = 0;
            var name = new StringBuilder(1024);
            var stringList = new List<string>();
            int error;
            do
            {
                uint classnamecount = 0;
                var zero = IntPtr.Zero;
                var capacity = (uint) name.Capacity;
                error = OffRegNativeMethods.OREnumKey(registryKey, dwIndex, name, ref capacity, null,
                    ref classnamecount, ref zero);
                if (error != 0)
                {
                    if (error != 259)
                        throw new Win32Exception(error);
                }
                else
                {
                    stringList.Add(name.ToString());
                    ++dwIndex;
                }
            } while (error != 259);

            return stringList.ToArray();
        }

        public static byte[] GetRawRegistrySecurity(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));
            uint size = 0;
            var num1 = 234;
            var keySecurity1 = OffRegNativeMethods.ORGetKeySecurity(handle,
                SecurityInformationFlags.DACL_SECURITY_INFORMATION |
                SecurityInformationFlags.SACL_SECURITY_INFORMATION | SecurityInformationFlags.MANDATORY_ACCESS_LABEL,
                null, ref size);
            var num2 = keySecurity1;
            if (num1 != num2)
                throw new Win32Exception(keySecurity1);
            var lpSecBuf = new byte[(int) size];
            var keySecurity2 = OffRegNativeMethods.ORGetKeySecurity(handle,
                SecurityInformationFlags.DACL_SECURITY_INFORMATION |
                SecurityInformationFlags.SACL_SECURITY_INFORMATION | SecurityInformationFlags.MANDATORY_ACCESS_LABEL,
                lpSecBuf, ref size);
            if (keySecurity2 != 0)
                throw new Win32Exception(keySecurity2);
            return lpSecBuf;
        }

        public static void SetRawRegistrySecurity(IntPtr handle, byte[] buf)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));
            var error = OffRegNativeMethods.ORSetKeySecurity(handle,
                SecurityInformationFlags.DACL_SECURITY_INFORMATION |
                SecurityInformationFlags.SACL_SECURITY_INFORMATION | SecurityInformationFlags.MANDATORY_ACCESS_LABEL,
                buf);
            if (error != 0)
                throw new Win32Exception(error);
        }

        public static RegistrySecurity GetRegistrySecurity(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));
            var registrySecurity1 = GetRawRegistrySecurity(handle);
            SecurityUtils.ConvertSDToStringSD(registrySecurity1,
                SecurityInformationFlags.SACL_SECURITY_INFORMATION | SecurityInformationFlags.MANDATORY_ACCESS_LABEL);
            var registrySecurity2 = new RegistrySecurity();
            registrySecurity2.SetSecurityDescriptorBinaryForm(registrySecurity1);
            return registrySecurity2;
        }

        public static int GetVirtualFlags(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));
            var pbFlags = 0;
            var virtualFlags = OffRegNativeMethods.ORGetVirtualFlags(handle, ref pbFlags);
            if (virtualFlags != 0)
                throw new Win32Exception(virtualFlags);
            return pbFlags;
        }

        public static int ExtractFromHive(string hivePath, RegistryValueType type, string targetPath)
        {
            if (string.IsNullOrEmpty(nameof(hivePath)))
                throw new ArgumentNullException(nameof(hivePath));
            if (string.IsNullOrEmpty(nameof(targetPath)))
                throw new ArgumentNullException(nameof(targetPath));
            if (!File.Exists(hivePath))
                throw new FileNotFoundException("Hive file {0} does not exist", hivePath);
            var num = 0;
            var flag = false;
            using (var srcHiveRoot = ORRegistryKey.OpenHive(hivePath))
            {
                using (var emptyHive = ORRegistryKey.CreateEmptyHive())
                {
                    flag = 0 < (num = ExtractFromHiveRecursive(srcHiveRoot, type, emptyHive));
                    if (flag)
                        emptyHive.SaveHive(targetPath);
                }

                if (flag)
                    srcHiveRoot.SaveHive(hivePath);
            }

            return num;
        }

        private static int ExtractFromHiveRecursive(ORRegistryKey srcHiveRoot, RegistryValueType type,
            ORRegistryKey dstHiveRoot)
        {
            var num = 0;
            var fullName = srcHiveRoot.FullName;
            foreach (var str in srcHiveRoot.ValueNameAndTypes.Where(p => p.Value == RegistryValueType.MultiString)
                .Select(q => q.Key))
            {
                var valueName = string.IsNullOrEmpty(str) ? null : str;
                var multiStringValue = srcHiveRoot.GetMultiStringValue(valueName);
                using (var subKey = dstHiveRoot.CreateSubKey(fullName))
                {
                    subKey.SetValue(valueName, multiStringValue);
                    ++num;
                }

                srcHiveRoot.DeleteValue(valueName);
            }

            foreach (var subKey in srcHiveRoot.SubKeys)
                using (var srcHiveRoot1 = srcHiveRoot.OpenSubKey(subKey))
                {
                    num += ExtractFromHiveRecursive(srcHiveRoot1, type, dstHiveRoot);
                }

            return num;
        }
    }
}