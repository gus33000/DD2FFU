// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.DataBlockEntry
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class DataBlockEntry
    {
        public DataBlockEntry(uint bytesPerBlock)
        {
            BytesPerBlock = bytesPerBlock;
            DataSource = new DataBlockSource();
        }

        private uint BytesPerBlock { get; }

        public List<DiskLocation> BlockLocationsOnDisk { get; private set; } = new List<DiskLocation>();

        public DataBlockSource DataSource { get; set; }

        public int SizeInBytes => 8 + BlockLocationsOnDisk.Count * DiskLocation.SizeInBytes;

        public void WriteEntryToStream(Stream outputStream)
        {
            var writer = new BinaryWriter(outputStream);
            writer.Write((uint) BlockLocationsOnDisk.Count);
            writer.Write(1);
            for (var index = 0; index < BlockLocationsOnDisk.Count; ++index)
                BlockLocationsOnDisk[index].Write(writer);
        }

        public void ReadEntryFromStream(BinaryReader reader, uint index)
        {
            var num = reader.ReadInt32();
            if (reader.ReadUInt32() != 1U)
                throw new ImageStorageException("More than one block per data block entry is not currently supported.");
            for (var index1 = 0; index1 < num; ++index1)
            {
                var diskLocation = new DiskLocation();
                diskLocation.Read(reader);
                BlockLocationsOnDisk.Add(diskLocation);
            }

            DataSource = new DataBlockSource
            {
                Source = DataBlockSource.DataSource.Disk,
                StorageOffset = index * BytesPerBlock
            };
        }

        public void WriteDataToByteArray(Stream sourceStream, byte[] blockData, int index, int byteCount)
        {
            var num = Math.Min(byteCount, (int) BytesPerBlock);
            switch (DataSource.Source)
            {
                case DataBlockSource.DataSource.Zero:
                    Array.Clear(blockData, index, num);
                    break;
                case DataBlockSource.DataSource.Disk:
                    sourceStream.Position = (long) DataSource.StorageOffset;
                    sourceStream.Read(blockData, index, num);
                    break;
                case DataBlockSource.DataSource.Memory:
                    Array.Copy(DataSource.GetMemoryData(), 0, blockData, index, num);
                    break;
            }
        }

        public void WriteDataToStream(Stream outputStream, Stream sourceStream, byte[] blockData)
        {
            WriteDataToByteArray(sourceStream, blockData, 0, (int) BytesPerBlock);
            outputStream.Write(blockData, 0, blockData.Length);
        }

        public void LogInfo(IULogger logger, bool logSources, ushort indentLevel = 0)
        {
            var str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Block Location Count: {0}", (object) BlockLocationsOnDisk.Count);
            foreach (var blockLocation in BlockLocationsOnDisk)
                blockLocation.LogInfo(logger, (ushort) (indentLevel + 2U));
            if (!logSources)
                return;
            DataSource.LogInfo(logger, (ushort) (indentLevel + 2U));
        }

        public DataBlockEntry CreateMemoryBlockEntry(Stream sourceStream)
        {
            var dataBlockEntry = new DataBlockEntry(BytesPerBlock);
            dataBlockEntry.BlockLocationsOnDisk = new List<DiskLocation>(BlockLocationsOnDisk);
            var newMemoryData = new DataBlockSource
            {
                Source = DataBlockSource.DataSource.Memory
            }.GetNewMemoryData(BytesPerBlock);
            WriteDataToByteArray(sourceStream, newMemoryData, 0, (int) BytesPerBlock);
            return dataBlockEntry;
        }
    }
}