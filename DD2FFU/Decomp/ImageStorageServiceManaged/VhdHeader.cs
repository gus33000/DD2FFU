// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.VhdHeader
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal struct VhdHeader
    {
        private const int VHD_HEADER_RESERVED_REGION_SIZE = 964;
        private const string VHD_HEADER_COOKIE = "cxsparse";
        private const uint VHD_HEADER_VERSION = 65536;
        public ulong Cookie;
        public ulong DataOffset;
        public ulong TableOffset;
        public uint HeaderVersion;
        public uint MaxTableEntries;
        public uint BlockSize;
        public uint CheckSum;
        public Guid ParentUniqueId;
        public uint ParentTimeStamp;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 964)]
        private byte[] Reserved;

        public VhdHeader(ulong vhdFileSize)
        {
            Cookie = BitConverter.ToUInt64(Encoding.ASCII.GetBytes("cxsparse"), 0);
            DataOffset = ulong.MaxValue;
            TableOffset = (ulong) (Marshal.SizeOf(typeof(VhdFooter)) + Marshal.SizeOf(typeof(VhdHeader)));
            HeaderVersion = 65536U;
            BlockSize = VhdCommon.DynamicVHDBlockSize;
            ParentUniqueId = Guid.Empty;
            ParentTimeStamp = 0U;
            Reserved = new byte[964];
            CheckSum = 0U;
            MaxTableEntries = 0U;
            MaxTableEntries = CalculateNumberOfBlocks(vhdFileSize);
            CheckSum = VhdCommon.CalculateChecksum(ref this);
        }

        private static uint CalculateNumberOfBlocks(ulong vhdFileSize)
        {
            if (vhdFileSize % VhdCommon.DynamicVHDBlockSize != 0UL)
                return (uint) (vhdFileSize / VhdCommon.DynamicVHDBlockSize + 1UL);
            return (uint) (vhdFileSize / VhdCommon.DynamicVHDBlockSize);
        }

        private void ChangeByteOrder()
        {
            DataOffset = VhdCommon.Swap64(DataOffset);
            TableOffset = VhdCommon.Swap64(TableOffset);
            HeaderVersion = VhdCommon.Swap32(HeaderVersion);
            MaxTableEntries = VhdCommon.Swap32(MaxTableEntries);
            BlockSize = VhdCommon.Swap32(BlockSize);
            CheckSum = VhdCommon.Swap32(CheckSum);
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

        public static VhdHeader Read(FileStream reader)
        {
            var vhdHeader = reader.ReadStruct<VhdHeader>();
            vhdHeader.ChangeByteOrder();
            return vhdHeader;
        }
    }
}