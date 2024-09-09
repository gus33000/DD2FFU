// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BlockAllocationTable
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.IO;
using System.Runtime.InteropServices;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class BlockAllocationTable
    {
        private readonly uint[] _blockAllocationTable;

        public BlockAllocationTable(uint batSize)
        {
            _blockAllocationTable =
                new uint[(int)(
                    VhdCommon.Round(batSize * (uint)Marshal.SizeOf(typeof(uint)), VhdCommon.VHDSectorSize) /
                    (uint)Marshal.SizeOf(typeof(uint)))];
            for (int index = 0; index < batSize; ++index)
            {
                _blockAllocationTable[index] = uint.MaxValue;
            }
        }

        public uint this[uint index]
        {
            get => _blockAllocationTable[(int)index];
            set => _blockAllocationTable[(int)index] = value;
        }

        public ulong SizeInBytes => (ulong)_blockAllocationTable.Length * (ulong)Marshal.SizeOf(typeof(uint));

        public uint EntryCount => (uint)_blockAllocationTable.Length;

        public void Write(FileStream writer)
        {
            foreach (uint data in _blockAllocationTable)
            {
                uint structure = VhdCommon.Swap32(data);
                writer.WriteStruct(ref structure);
            }
        }

        public void Read(FileStream reader)
        {
            for (int index = 0; index < _blockAllocationTable.Length; ++index)
            {
                uint data = reader.ReadStruct<uint>();
                _blockAllocationTable[index] = VhdCommon.Swap32(data);
            }
        }
    }
}