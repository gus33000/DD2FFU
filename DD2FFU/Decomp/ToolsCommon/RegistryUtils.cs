// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.RegistryUtils
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public static class RegistryUtils
    {
        private const string STR_REG_LOAD = "LOAD {0} {1}";
        private const string STR_REG_EXPORT = "EXPORT {0} {1}";
        private const string STR_REG_UNLOAD = "UNLOAD {0}";
        private const string STR_REGEXE = "%windir%\\System32\\REG.EXE";

        private static readonly Dictionary<string, SystemRegistryHiveFiles> hiveMap =
            new(StringComparer.InvariantCultureIgnoreCase)
            {
                {
                    "SOFTWARE",
                    SystemRegistryHiveFiles.SOFTWARE
                },
                {
                    "SYSTEM",
                    SystemRegistryHiveFiles.SYSTEM
                },
                {
                    "DRIVERS",
                    SystemRegistryHiveFiles.DRIVERS
                },
                {
                    "DEFAULT",
                    SystemRegistryHiveFiles.DEFAULT
                },
                {
                    "SAM",
                    SystemRegistryHiveFiles.SAM
                },
                {
                    "SECURITY",
                    SystemRegistryHiveFiles.SECURITY
                },
                {
                    "BCD",
                    SystemRegistryHiveFiles.BCD
                },
                {
                    "NTUSER.DAT",
                    SystemRegistryHiveFiles.CURRENTUSER
                }
            };

        public static Dictionary<SystemRegistryHiveFiles, string> KnownMountPoints
        {
            get;
        } =
            new Dictionary<SystemRegistryHiveFiles, string>
            {
                {
                    SystemRegistryHiveFiles.SOFTWARE,
                    "HKEY_LOCAL_MACHINE\\SOFTWARE"
                },
                {
                    SystemRegistryHiveFiles.SYSTEM,
                    "HKEY_LOCAL_MACHINE\\SYSTEM"
                },
                {
                    SystemRegistryHiveFiles.DRIVERS,
                    "HKEY_LOCAL_MACHINE\\DRIVERS"
                },
                {
                    SystemRegistryHiveFiles.DEFAULT,
                    "HKEY_USERS\\.DEFAULT"
                },
                {
                    SystemRegistryHiveFiles.SAM,
                    "HKEY_LOCAL_MACHINE\\SAM"
                },
                {
                    SystemRegistryHiveFiles.SECURITY,
                    "HKEY_LOCAL_MACHINE\\SECURITY"
                },
                {
                    SystemRegistryHiveFiles.BCD,
                    "HKEY_LOCAL_MACHINE\\BCD"
                },
                {
                    SystemRegistryHiveFiles.CURRENTUSER,
                    "HKEY_CURRENT_USER"
                }
            };

        public static void ConvertSystemHiveToRegFile(DriveInfo systemDrive, SystemRegistryHiveFiles hive,
            string outputRegFile)
        {
            LongPathDirectory.CreateDirectory(LongPath.GetDirectoryName(outputRegFile));
            ConvertHiveToRegFile(
                Path.Combine(Path.Combine(systemDrive.RootDirectory.FullName, "windows\\system32\\config"),
                    Enum.GetName(typeof(SystemRegistryHiveFiles), hive)), MapHiveToMountPoint(hive), outputRegFile);
        }

        public static void ConvertHiveToRegFile(string inputhive, string targetRootKey, string outputRegFile)
        {
            OfflineRegUtils.ConvertHiveToReg(inputhive, outputRegFile, targetRootKey);
        }

        public static void LoadHive(string inputhive, string mountpoint)
        {
            int error = CommonUtils.RunProcess(Environment.CurrentDirectory,
                Environment.ExpandEnvironmentVariables("%windir%\\System32\\REG.EXE"),
                string.Format("LOAD {0} {1}", mountpoint, inputhive), true);
            if (0 < error)
            {
                throw new Win32Exception(error);
            }

            Thread.Sleep(500);
        }

        public static void ExportHive(string mountpoint, string outputfile)
        {
            int error = CommonUtils.RunProcess(Environment.CurrentDirectory,
                Environment.ExpandEnvironmentVariables("%windir%\\System32\\REG.EXE"),
                string.Format("EXPORT {0} {1}", mountpoint, outputfile), true);
            if (0 < error)
            {
                throw new Win32Exception(error);
            }

            Thread.Sleep(500);
        }

        public static void UnloadHive(string mountpoint)
        {
            int error = CommonUtils.RunProcess(Environment.CurrentDirectory,
                Environment.ExpandEnvironmentVariables("%windir%\\System32\\REG.EXE"),
                string.Format("UNLOAD {0}", mountpoint), true);
            if (0 < error)
            {
                throw new Win32Exception(error);
            }
        }

        public static string MapHiveToMountPoint(SystemRegistryHiveFiles hive)
        {
            return KnownMountPoints[hive];
        }

        public static string MapHiveFileToMountPoint(string hiveFile)
        {
            return string.IsNullOrEmpty(hiveFile)
                ? throw new InvalidOperationException("hiveFile cannot be empty")
                : !hiveMap.TryGetValue(Path.GetFileName(hiveFile), out SystemRegistryHiveFiles hive) ? "" : MapHiveToMountPoint(hive);
        }
    }
}