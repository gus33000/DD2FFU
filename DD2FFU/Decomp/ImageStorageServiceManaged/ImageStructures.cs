// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.ImageStructures
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Runtime.InteropServices;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class ImageStructures
    {
        [Flags]
        public enum DiskAttributes : ulong
        {
            Offline = 1,
            ReadOnly = 2
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct PARTITION_ENTRY
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)] [FieldOffset(0)]
            private string name;

            [FieldOffset(72)] private ulong sectorCount;
            [FieldOffset(80)] private uint alignmentSizeInBytes;
            [FieldOffset(84)] private uint clusterSize;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] [FieldOffset(88)]
            private string fileSystem;

            [FieldOffset(152)] private Guid id;
            [FieldOffset(168)] private Guid type;
            [FieldOffset(184)] private ulong flags;
            [FieldOffset(152)] private byte mbrFlags;
            [FieldOffset(153)] private byte mbrType;
            [FieldOffset(192)] private ulong offsetInSectors;

            public string PartitionName
            {
                get => name;
                set => name = value;
            }

            
            public ulong SectorCount
            {
                get => sectorCount;
                set => sectorCount = value;
            }

            
            public uint AlignmentSizeInBytes
            {
                get => alignmentSizeInBytes;
                set => alignmentSizeInBytes = value;
            }

            
            public uint ClusterSize
            {
                get => clusterSize;
                set => clusterSize = value;
            }

            public string FileSystem
            {
                get => fileSystem;
                set => fileSystem = value;
            }

            public Guid PartitionId
            {
                get => id;
                set => id = value;
            }

            public Guid PartitionType
            {
                get => type;
                set => type = value;
            }

            
            public ulong PartitionFlags
            {
                get => flags;
                set => flags = value;
            }

            public byte MBRFlags
            {
                get => mbrFlags;
                set => mbrFlags = value;
            }

            public byte MBRType
            {
                get => mbrType;
                set => mbrType = value;
            }

            
            public ulong OffsetInSectors
            {
                get => offsetInSectors;
                set => offsetInSectors = value;
            }
        }

        
        [StructLayout(LayoutKind.Explicit)]
        public struct STORE_ID
        {
            [FieldOffset(0)] private uint storeType;
            [FieldOffset(4)] private Guid storeId_GPT;
            [FieldOffset(4)] private uint storeId_MBR;

            public uint StoreType
            {
                get => storeType;
                set => storeType = value;
            }

            public Guid StoreId_GPT
            {
                get => storeId_GPT;
                set => storeId_GPT = value;
            }

            public uint StoreId_MBR
            {
                get => storeId_MBR;
                set => storeId_MBR = value;
            }

            public bool IsEmpty
            {
                get
                {
                    if (storeId_GPT == Guid.Empty)
                        return storeId_MBR == 0U;
                    return false;
                }
            }
        }

        
        [StructLayout(LayoutKind.Explicit)]
        public struct PartitionAttributes
        {
            [FieldOffset(0)] public ulong gptAttributes;
            [FieldOffset(0)] public byte mbrAttributes;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct PartitionType
        {
            [FieldOffset(0)] public Guid gptType;
            [FieldOffset(0)] public byte mbrType;
        }

        public struct SetDiskAttributes
        {
            public uint Version;
            public byte Persist;
            private byte dummy1;
            private byte dummy2;
            private byte dummy3;
            public DiskAttributes Attributes;
            public DiskAttributes AttributesMask;
            private Guid Reserved;
        }
    }
}