// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BuildPaths
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagecommon.dll

using System;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BuildPaths
    {
        public static string OEMKitFMSchema => "OEMKitFM.xsd";

        public static string PropsProjectSchema => "PropsProject.xsd";

        public static string PropsGuidMappingsSchema => "PropsGuidMappings.xsd";

        public static string PublishingPackageInfoSchema => "PublishingPackageInfo.xsd";

        public static string FMCollectionSchema => "FMCollection.xsd";

        public static string BuildCompDBSchema => "BuildCompDB.xsd";

        public static string UpdateCompDBSchema => "UpdateCompDB.xsd";

        public static string BSPCompDBSchema => "BSPCompDB.xsd";

        public static string DeviceCompDBSchema => "DeviceCompDB.xsd";

        public static string CompDBChunkMappingSchema => "CompDBChunkMapping.xsd";

        public static string CompDBPublishingInfoSchema => "CompDBPublishingInfo.xsd";

        public static string GetImagingTempPath(string defaultPath)
        {
            var environmentVariable = Environment.GetEnvironmentVariable("BUILD_PRODUCT");
            var dir = Environment.GetEnvironmentVariable("OBJECT_ROOT");
            if (!string.IsNullOrEmpty(environmentVariable) &&
                environmentVariable.Equals("nt", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(dir))
            {
                dir = Environment.GetEnvironmentVariable("TEMP");
                if (string.IsNullOrEmpty(dir))
                {
                    dir = Environment.GetEnvironmentVariable("TMP");
                    if (string.IsNullOrEmpty(dir))
                        dir = defaultPath;
                }
            }

            return FileUtils.GetTempFile(dir);
        }
    }
}