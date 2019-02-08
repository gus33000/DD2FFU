// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.VhdFooter
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct VhdFooter
    {
        private const string VHD_FOOTER_COOKIE = "conectix";
        private const uint VHD_FILE_FORMAT_VERSION = 65536;
        private const uint VHD_FOOTER_CREATOR_APPLICATION = 1987278701;
        private const uint VHD_FOOTER_CREATOR_VERSION = 65536;
        private const int VHD_FOOTER_RESERVED_REGION_SIZE = 427;
        public ulong Cookie;
        public uint Features;
        public uint FileFormatVersion;
        public ulong DataOffset;
        public uint TimeStamp;
        public uint CreatorApplication;
        public uint CreatorVersion;
        public uint CreatorHostOs;
        public ulong OriginalSize;
        public ulong CurrentSize;
        private uint DriveGeometry;
        private uint DriveType;
        private uint CheckSum;
        private readonly Guid UniqueId;
        private readonly byte SavedState;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 427)]
        private readonly byte[] Reserved;

        public VhdFooter(ulong vhdFileSize, VhdType vhdType, ulong dataOffset)
        {
            Cookie = BitConverter.ToUInt64(Encoding.ASCII.GetBytes("conectix"), 0);
            Features = 0U;
            FileFormatVersion = 65536U;
            DataOffset = dataOffset;
            TimeStamp = (uint) (DateTime.UtcNow - new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            CreatorApplication = 1987278701U;
            CreatorVersion = 65536U;
            CreatorHostOs = 1466511979U;
            OriginalSize = vhdFileSize;
            CurrentSize = vhdFileSize;
            UniqueId = new Guid();
            DriveType = (uint) vhdType;
            SavedState = 0;
            Reserved = new byte[427];
            DriveGeometry = GetDriveGeometry(vhdFileSize);
            CheckSum = 0U;
            CheckSum = VhdCommon.CalculateChecksum(ref this);
        }

        public VhdFooter(ulong vhdFileSize, VhdType type)
        {
            this = new VhdFooter(vhdFileSize, type, ulong.MaxValue);
        }

        public void Write(FileStream writer)
        {
            ChangeByteOrder();
            try
            {
                writer.WriteStruct(ref this);
            }
            finally
            {
                ChangeByteOrder();
                writer.Flush();
            }
        }

        public static VhdFooter Read(FileStream reader)
        {
            var vhdFooter = reader.ReadStruct<VhdFooter>();
            vhdFooter.ChangeByteOrder();
            return vhdFooter;
        }

        private void ChangeByteOrder()
        {
            Features = VhdCommon.Swap32(Features);
            FileFormatVersion = VhdCommon.Swap32(FileFormatVersion);
            DataOffset = VhdCommon.Swap64(DataOffset);
            TimeStamp = VhdCommon.Swap32(TimeStamp);
            CreatorApplication = VhdCommon.Swap32(CreatorApplication);
            CreatorVersion = VhdCommon.Swap32(CreatorVersion);
            CreatorHostOs = VhdCommon.Swap32(CreatorHostOs);
            OriginalSize = VhdCommon.Swap64(OriginalSize);
            CurrentSize = VhdCommon.Swap64(CurrentSize);
            DriveGeometry = VhdCommon.Swap32(DriveGeometry);
            DriveType = VhdCommon.Swap32(DriveType);
            CheckSum = VhdCommon.Swap32(CheckSum);
        }

        private static uint GetDriveGeometry(ulong vhdFileSize)
        {
            var num1 = (uint) (vhdFileSize / VhdCommon.VHDSectorSize);
            var num2 = 0;
            if (num1 > 267382800U)
                num1 = 267382800U;
            uint num3;
            uint num4;
            uint num5;
            if (num1 >= 66059280U)
            {
                num3 = byte.MaxValue;
                num4 = 16U;
                num5 = num1 / num3 / num4;
            }
            else
            {
                num3 = 17U;
                var num6 = num1 / num3;
                num4 = (num6 + 1023U) / 1024U;
                if (num4 < 4U)
                    num4 = 4U;
                if (num6 >= num4 * 1024U || num4 > 16U)
                {
                    num3 = 31U;
                    num4 = 16U;
                    num6 = num1 / num3;
                }

                if (num6 >= num4 * 1024U)
                {
                    num3 = 63U;
                    num4 = 16U;
                    num6 = num1 / num3;
                }

                num5 = num6 / num4;
            }

            var num7 = (int) num5 << 16;
            return (uint) (num2 | num7 | ((int) num4 << 8)) | num3;
        }

        private enum VhdFooterFeatures : uint
        {
            VHD_FOOTER_FEATURES_NONE,
            VHD_FOOTER_FEATURES_TEMPORARY,
            VHD_FOOTER_FEATURES_RESERVED
        }

        private enum VhdFooterCreaterHostOS : uint
        {
            VHD_FOOTER_CREATOR_HOST_OS_MACINTOSH = 1298228000, // 0x4D616320
            VHD_FOOTER_CREATOR_HOST_OS_WINDOWS = 1466511979 // 0x5769326B
        }
    }
}