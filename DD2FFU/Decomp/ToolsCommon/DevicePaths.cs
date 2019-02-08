// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.DevicePaths
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System.IO;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public class DevicePaths
    {
        public const string MAINOS_PARTITION_NAME = "MainOS";
        public const string MMOS_PARTITION_NAME = "MMOS";
        private static readonly string _updateFilesPath = "SharedData\\DuShared";
        private static readonly string _BiosBCDHivePath = "boot";
        private static readonly string _UefiBCDHivePath = "efi\\Microsoft\\boot";
        private static string _dsmPath = ImageUpdatePath;
        private static readonly string _UpdateOSPath = "PROGRAMS\\UpdateOS\\";
        private static readonly string _FMFilesDirectory = "FeatureManifest";
        private static readonly string _OEMInputPath = "OEMInput";
        private static readonly string _OEMInputFile = "OEMInput.xml";

        public static string ImageUpdatePath { get; } = "Windows\\ImageUpdate";

        public static string DeviceLayoutFileName { get; } = "DeviceLayout.xml";

        public static string DeviceLayoutFilePath => Path.Combine(ImageUpdatePath, DeviceLayoutFileName);

        public static string OemDevicePlatformFileName { get; } = "OEMDevicePlatform.xml";

        public static string OemDevicePlatformFilePath => Path.Combine(ImageUpdatePath, OemDevicePlatformFileName);

        public static string UpdateOutputFile { get; } = "UpdateOutput.xml";

        public static string UpdateOutputFilePath => Path.Combine(_updateFilesPath, UpdateOutputFile);

        public static string UpdateHistoryFile { get; } = "UpdateHistory.xml";

        public static string UpdateHistoryFilePath => Path.Combine(ImageUpdatePath, UpdateHistoryFile);

        public static string UpdateOSWIMName { get; } = "UpdateOS.wim";

        public static string UpdateOSWIMFilePath => Path.Combine(_UpdateOSPath, UpdateOSWIMName);

        public static string MMOSWIMName { get; } = "MMOS.wim";

        public static string MMOSWIMFilePath => MMOSWIMName;

        public static string RegistryHivePath { get; } = "Windows\\System32\\Config";

        public static string DeviceLayoutSchema => "DeviceLayout.xsd";

        public static string DeviceLayoutSchema2 => "DeviceLayoutv2.xsd";

        public static string UpdateOSInputSchema => "UpdateOSInput.xsd";

        public static string OEMInputSchema => "OEMInput.xsd";

        public static string FeatureManifestSchema => "FeatureManifest.xsd";

        public static string MicrosoftPhoneSKUSchema => "MicrosoftPhoneSKU.xsd";

        public static string UpdateOSOutputSchema => "UpdateOSOutput.xsd";

        public static string UpdateHistorySchema => "UpdateHistory.xsd";

        public static string OEMDevicePlatformSchema => "OEMDevicePlatform.xsd";

        public static string MSFMPath => Path.Combine(ImageUpdatePath, _FMFilesDirectory, "Microsoft");

        public static string MSFMPathOld => ImageUpdatePath;

        public static string OEMFMPath => Path.Combine(ImageUpdatePath, _FMFilesDirectory, "OEM");

        public static string OEMInputPath => Path.Combine(ImageUpdatePath, _OEMInputPath);

        public static string OEMInputFile => Path.Combine(OEMInputPath, _OEMInputFile);

        public static string GetBCDHivePath(bool isUefiBoot)
        {
            if (!isUefiBoot)
                return _BiosBCDHivePath;
            return _UefiBCDHivePath;
        }

        public static string GetRegistryHiveFilePath(SystemRegistryHiveFiles hiveType)
        {
            return GetRegistryHiveFilePath(hiveType, true);
        }

        public static string GetRegistryHiveFilePath(SystemRegistryHiveFiles hiveType, bool isUefiBoot)
        {
            var str = "";
            switch (hiveType)
            {
                case SystemRegistryHiveFiles.SYSTEM:
                    str = Path.Combine(RegistryHivePath, "SYSTEM");
                    break;
                case SystemRegistryHiveFiles.SOFTWARE:
                    str = Path.Combine(RegistryHivePath, "SOFTWARE");
                    break;
                case SystemRegistryHiveFiles.DEFAULT:
                    str = Path.Combine(RegistryHivePath, "DEFAULT");
                    break;
                case SystemRegistryHiveFiles.DRIVERS:
                    str = Path.Combine(RegistryHivePath, "DRIVERS");
                    break;
                case SystemRegistryHiveFiles.SAM:
                    str = Path.Combine(RegistryHivePath, "SAM");
                    break;
                case SystemRegistryHiveFiles.SECURITY:
                    str = Path.Combine(RegistryHivePath, "SECURITY");
                    break;
                case SystemRegistryHiveFiles.BCD:
                    str = Path.Combine(GetBCDHivePath(isUefiBoot), "BCD");
                    break;
            }

            return str;
        }
    }
}