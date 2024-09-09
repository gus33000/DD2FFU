// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.SecurityUtils
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public class SecurityUtils
    {
        private const int ERROR_INSUFFICIENT_BUFFER = 122;
        private static readonly Regex regexExtractMIL = new("(?<MIL>\\(ML[^\\)]*\\))", RegexOptions.Compiled);

        public static string GetFileSystemMandatoryLevel(string resourcePath)
        {
            string empty = string.Empty;
            string stringSd =
                ConvertSDToStringSD(
                    GetSecurityDescriptor(resourcePath, SecurityInformationFlags.MANDATORY_ACCESS_LABEL),
                    SecurityInformationFlags.MANDATORY_ACCESS_LABEL);
            if (!string.IsNullOrEmpty(stringSd))
            {
                string input = stringSd.TrimEnd(new char[1]);
                Match match = regexExtractMIL.Match(input);
                if (match.Success)
                {
                    empty = match.Groups["MIL"].Value;
                }
            }

            return empty;
        }


        public static byte[] GetSecurityDescriptor(string resourcePath, SecurityInformationFlags flags)
        {
            byte[] destination = null;
            int lpnLengthNeeded = 0;
            _ = NativeSecurityMethods.GetFileSecurity(resourcePath, flags, nint.Zero, 0, ref lpnLengthNeeded);
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error != 122)
            {
                Console.WriteLine("Error {0} Calling GetFileSecurity() on {1}", lastWin32Error, resourcePath);
                throw new Win32Exception(lastWin32Error);
            }

            int num1 = lpnLengthNeeded;
            nint num2 = Marshal.AllocHGlobal(num1);
            try
            {
                if (!NativeSecurityMethods.GetFileSecurity(resourcePath, flags, num2, num1, ref lpnLengthNeeded))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                destination = new byte[lpnLengthNeeded];
                Marshal.Copy(num2, destination, 0, lpnLengthNeeded);
            }
            finally
            {
                Marshal.FreeHGlobal(num2);
            }

            return destination;
        }


        public static string ConvertSDToStringSD(byte[] securityDescriptor, SecurityInformationFlags flags)
        {
            bool securityDescriptor1 = NativeSecurityMethods.ConvertSecurityDescriptorToStringSecurityDescriptor(
    securityDescriptor, 1, flags, out nint StringSecurityDescriptor, out int StringSecurityDescriptorLen);
            try
            {
                return !securityDescriptor1
                    ? throw new Win32Exception(Marshal.GetLastWin32Error())
                    : Marshal.PtrToStringUni(StringSecurityDescriptor, StringSecurityDescriptorLen);
            }
            finally
            {
                if (StringSecurityDescriptor != nint.Zero)
                {
                    Marshal.FreeHGlobal(StringSecurityDescriptor);
                }
            }
        }

        public static AclCollection GetFileSystemACLs(string rootDir)
        {
            if (rootDir == null)
            {
                throw new ArgumentNullException(nameof(rootDir));
            }

            if (!LongPathDirectory.Exists(rootDir))
            {
                throw new ArgumentException(string.Format("Directory {0} does not exist", rootDir));
            }

            AclCollection accesslist = [];
            DirectoryInfo directoryInfo = new(rootDir);
            DirectoryAcl directoryAcl = new(directoryInfo, rootDir);
            if (!directoryAcl.IsEmpty)
            {
                _ = accesslist.Add(directoryAcl);
            }

            GetFileSystemACLsRecursive(directoryInfo, rootDir, accesslist);
            return accesslist;
        }

        public static AclCollection GetRegistryACLs(string hiveRoot)
        {
            if (hiveRoot == null)
            {
                throw new ArgumentNullException(nameof(hiveRoot));
            }

            if (!LongPathDirectory.Exists(hiveRoot))
            {
                throw new ArgumentException(string.Format("Directory {0} does not exist", hiveRoot));
            }

            AclCollection accesslist = [];
            foreach (SystemRegistryHiveFiles hive in Enum.GetValues(typeof(SystemRegistryHiveFiles)))
            {
                using ORRegistryKey parent = ORRegistryKey.OpenHive(
                                Path.Combine(hiveRoot, Enum.GetName(typeof(SystemRegistryHiveFiles), hive)),
                                RegistryUtils.MapHiveToMountPoint(hive));
                GetRegistryACLsRecursive(parent, accesslist);
            }

            return accesslist;
        }

        private static void GetFileSystemACLsRecursive(DirectoryInfo rootdi, string rootDir, AclCollection accesslist)
        {
            foreach (DirectoryInfo directory in rootdi.GetDirectories())
            {
                GetFileSystemACLsRecursive(directory, rootDir, accesslist);
                DirectoryAcl directoryAcl = new(directory, rootDir);
                if (!directoryAcl.IsEmpty)
                {
                    _ = accesslist.Add(directoryAcl);
                }
            }

            foreach (FileInfo file in rootdi.GetFiles())
            {
                FileAcl fileAcl = new(file, rootDir);
                if (!fileAcl.IsEmpty)
                {
                    _ = accesslist.Add(fileAcl);
                }
            }
        }

        public static void GetRegistryACLsRecursive(ORRegistryKey parent, AclCollection accesslist)
        {
            foreach (string subKey in parent.SubKeys)
            {
                using ORRegistryKey orRegistryKey = parent.OpenSubKey(subKey);
                GetRegistryACLsRecursive(orRegistryKey, accesslist);
                RegistryAcl registryAcl = new(orRegistryKey);
                if (!registryAcl.IsEmpty)
                {
                    _ = accesslist.Add(registryAcl);
                }
            }
        }
    }
}