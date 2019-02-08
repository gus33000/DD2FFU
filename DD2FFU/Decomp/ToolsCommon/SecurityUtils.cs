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
        private static readonly Regex regexExtractMIL = new Regex("(?<MIL>\\(ML[^\\)]*\\))", RegexOptions.Compiled);

        public static string GetFileSystemMandatoryLevel(string resourcePath)
        {
            var empty = string.Empty;
            var stringSd =
                ConvertSDToStringSD(
                    GetSecurityDescriptor(resourcePath, SecurityInformationFlags.MANDATORY_ACCESS_LABEL),
                    SecurityInformationFlags.MANDATORY_ACCESS_LABEL);
            if (!string.IsNullOrEmpty(stringSd))
            {
                var input = stringSd.TrimEnd(new char[1]);
                var match = regexExtractMIL.Match(input);
                if (match.Success)
                    empty = match.Groups["MIL"].Value;
            }

            return empty;
        }

        
        public static byte[] GetSecurityDescriptor(string resourcePath, SecurityInformationFlags flags)
        {
            byte[] destination = null;
            var lpnLengthNeeded = 0;
            NativeSecurityMethods.GetFileSecurity(resourcePath, flags, IntPtr.Zero, 0, ref lpnLengthNeeded);
            var lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error != 122)
            {
                Console.WriteLine("Error {0} Calling GetFileSecurity() on {1}", lastWin32Error, resourcePath);
                throw new Win32Exception(lastWin32Error);
            }

            var num1 = lpnLengthNeeded;
            var num2 = Marshal.AllocHGlobal(num1);
            try
            {
                if (!NativeSecurityMethods.GetFileSecurity(resourcePath, flags, num2, num1, ref lpnLengthNeeded))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
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
            var empty = string.Empty;
            IntPtr StringSecurityDescriptor;
            int StringSecurityDescriptorLen;
            var securityDescriptor1 = NativeSecurityMethods.ConvertSecurityDescriptorToStringSecurityDescriptor(
                securityDescriptor, 1, flags, out StringSecurityDescriptor, out StringSecurityDescriptorLen);
            try
            {
                if (!securityDescriptor1)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                return Marshal.PtrToStringUni(StringSecurityDescriptor, StringSecurityDescriptorLen);
            }
            finally
            {
                if (StringSecurityDescriptor != IntPtr.Zero)
                    Marshal.FreeHGlobal(StringSecurityDescriptor);
                var zero = IntPtr.Zero;
            }
        }

        public static AclCollection GetFileSystemACLs(string rootDir)
        {
            if (rootDir == null)
                throw new ArgumentNullException(nameof(rootDir));
            if (!LongPathDirectory.Exists(rootDir))
                throw new ArgumentException(string.Format("Directory {0} does not exist", rootDir));
            var accesslist = new AclCollection();
            var directoryInfo = new DirectoryInfo(rootDir);
            var directoryAcl = new DirectoryAcl(directoryInfo, rootDir);
            if (!directoryAcl.IsEmpty)
                accesslist.Add(directoryAcl);
            GetFileSystemACLsRecursive(directoryInfo, rootDir, accesslist);
            return accesslist;
        }

        public static AclCollection GetRegistryACLs(string hiveRoot)
        {
            if (hiveRoot == null)
                throw new ArgumentNullException(nameof(hiveRoot));
            if (!LongPathDirectory.Exists(hiveRoot))
                throw new ArgumentException(string.Format("Directory {0} does not exist", hiveRoot));
            var accesslist = new AclCollection();
            foreach (SystemRegistryHiveFiles hive in Enum.GetValues(typeof(SystemRegistryHiveFiles)))
                using (var parent = ORRegistryKey.OpenHive(
                    Path.Combine(hiveRoot, Enum.GetName(typeof(SystemRegistryHiveFiles), hive)),
                    RegistryUtils.MapHiveToMountPoint(hive)))
                {
                    GetRegistryACLsRecursive(parent, accesslist);
                }

            return accesslist;
        }

        private static void GetFileSystemACLsRecursive(DirectoryInfo rootdi, string rootDir, AclCollection accesslist)
        {
            foreach (var directory in rootdi.GetDirectories())
            {
                GetFileSystemACLsRecursive(directory, rootDir, accesslist);
                var directoryAcl = new DirectoryAcl(directory, rootDir);
                if (!directoryAcl.IsEmpty)
                    accesslist.Add(directoryAcl);
            }

            foreach (var file in rootdi.GetFiles())
            {
                var fileAcl = new FileAcl(file, rootDir);
                if (!fileAcl.IsEmpty)
                    accesslist.Add(fileAcl);
            }
        }

        public static void GetRegistryACLsRecursive(ORRegistryKey parent, AclCollection accesslist)
        {
            foreach (var subKey in parent.SubKeys)
                using (var orRegistryKey = parent.OpenSubKey(subKey))
                {
                    GetRegistryACLsRecursive(orRegistryKey, accesslist);
                    var registryAcl = new RegistryAcl(orRegistryKey);
                    if (!registryAcl.IsEmpty)
                        accesslist.Add(registryAcl);
                }
        }
    }
}