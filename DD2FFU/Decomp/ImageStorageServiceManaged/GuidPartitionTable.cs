// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.GuidPartitionTable
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class GuidPartitionTable
    {
        private const int MIN_GPT_PARTITION_ARRAY_SIZE = 16384;

        public GuidPartitionTable(int bytesPerSector, IULogger logger)
        {
            Logger = logger;
            BytesPerSector = bytesPerSector;
        }

        public int BytesPerSector { get; set; }

        public MasterBootRecord ProtectiveMbr { get; private set; }

        public GuidPartitionTableHeader Header { get; private set; }

        public List<GuidPartitionTableEntry> Entries { get; private set; }

        private IULogger Logger { get; }

        public void ReadFromStream(Stream stream, bool readPrimaryTable)
        {
            long num = BytesPerSector;
            if (BytesPerSector == 0)
                throw new ImageStorageException(
                    "BytesPerSector must be initialized before calling GuidPartitionTable.ReadFromStream.");
            if (readPrimaryTable)
            {
                ProtectiveMbr = new MasterBootRecord(Logger, BytesPerSector);
                ProtectiveMbr.ReadFromStream(stream, MasterBootRecord.MbrParseType.Normal);
            }
            else
            {
                num = stream.Length - BytesPerSector;
            }

            stream.Position = num;
            Header = new GuidPartitionTableHeader(Logger);
            Header.ReadFromStream(stream, BytesPerSector);
            stream.Position = (long) Header.PartitionEntryStartSector * BytesPerSector;
            var capacity = (int) Math.Max(Header.PartitionEntryCount, 16384U / Header.PartitionEntrySizeInBytes);
            Entries = new List<GuidPartitionTableEntry>(capacity);
            for (var index = 0; index < capacity; ++index)
            {
                var partitionTableEntry = new GuidPartitionTableEntry(Logger);
                partitionTableEntry.ReadFromStream(stream, (int) Header.PartitionEntrySizeInBytes);
                Entries.Add(partitionTableEntry);
            }
        }

        public void WriteToStream(Stream stream, bool fPrimaryTable, bool onlyAllocateDefinedGptEntries)
        {
            var num = (long) BytesPerSector;
            if (fPrimaryTable)
            {
                if (ProtectiveMbr == null)
                    throw new ImageStorageException("The GuidPartitionTable protective MBR is null.");
                ProtectiveMbr.WriteToStream(stream, false);
            }
            else
            {
                num = stream.Length - BytesPerSector;
            }

            if (Header == null)
                throw new ImageStorageException("The GuidPartitionTable header is null.");
            stream.Position = num;
            Header.PartitionEntryCount = !onlyAllocateDefinedGptEntries
                ? Math.Max(Header.PartitionEntryCount, 16384U / Header.PartitionEntrySizeInBytes)
                : 4096U / Header.PartitionEntrySizeInBytes;
            Header.PartitionEntryArrayCrc32 = ComputePartitionEntryCrc(Header.PartitionEntryCount);
            Header.FixHeaderCrc(BytesPerSector);
            Header.WriteToStream(stream, BytesPerSector);
            stream.Position = (long) Header.PartitionEntryStartSector * BytesPerSector;
            foreach (var entry in Entries)
                entry.WriteToStream(stream, (int) Header.PartitionEntrySizeInBytes);
        }

        public void LogInfo(ushort indentLevel = 0)
        {
            ProtectiveMbr.LogInfo(Logger, indentLevel);
            Header.LogInfo(indentLevel);
            Logger.LogInfo("");
            Logger.LogInfo("Partition Entry Array");
            foreach (var entry in Entries)
                entry.LogInfo((ushort) (indentLevel + 2U));
        }

        public Guid SetEntryId(string partitionName, Guid partitionId)
        {
            var guid = Guid.Empty;
            foreach (var entry in Entries)
                if (string.CompareOrdinal(entry.PartitionName.Split(new char[1])[0], partitionName) == 0)
                {
                    guid = entry.PartitionId;
                    entry.PartitionId = partitionId;
                    break;
                }

            return guid;
        }

        public void RemoveEntry(string partitionName)
        {
            foreach (var entry in Entries)
                if (string.CompareOrdinal(entry.PartitionName.Split(new char[1])[0], partitionName) == 0)
                {
                    entry.Clean();
                    break;
                }
        }

        public uint ComputePartitionEntryCrc()
        {
            return ComputePartitionEntryCrc(Header.PartitionEntryCount);
        }

        public uint ComputePartitionEntryCrc(uint partitionEntryCount)
        {
            var crC32 = new CRC32();
            var memoryStream = new MemoryStream();
            uint num = 0;
            foreach (var entry in Entries)
                if (++num <= partitionEntryCount)
                    entry.WriteToStream(memoryStream, (int) Header.PartitionEntrySizeInBytes);
                else
                    break;
            var hash = crC32.ComputeHash(memoryStream.GetBuffer());
            return (uint) ((hash[0] << 24) | (hash[1] << 16) | (hash[2] << 8)) | hash[3];
        }

        public void ValidatePartitionEntryCrc()
        {
            var partitionEntryCrc = ComputePartitionEntryCrc();
            if ((int) Header.PartitionEntryArrayCrc32 != (int) partitionEntryCrc)
                throw new ImageStorageException(string.Format(
                    "The partition entry array CRC is invalid.  Actual: {0:x} Expected: {1:x}.",
                    Header.PartitionEntryArrayCrc32, partitionEntryCrc));
        }

        public void NormalizeGptIds(out Guid originalSystemPartitionId)
        {
            Header.DiskId = ImageConstants.SYSTEM_STORE_GUID;
            SetEntryId(ImageConstants.MAINOS_PARTITION_NAME, ImageConstants.MAINOS_PARTITION_ID);
            SetEntryId(ImageConstants.MMOS_PARTITION_NAME, ImageConstants.MMOS_PARTITION_ID);
            originalSystemPartitionId =
                SetEntryId(ImageConstants.SYSTEM_PARTITION_NAME, ImageConstants.SYSTEM_PARTITION_ID);
        }

        public void RandomizeGptIds()
        {
            Header.DiskId = Guid.NewGuid();
            SetEntryId(ImageConstants.MAINOS_PARTITION_NAME, Guid.NewGuid());
            SetEntryId(ImageConstants.MMOS_PARTITION_NAME, Guid.NewGuid());
            SetEntryId(ImageConstants.SYSTEM_PARTITION_NAME, Guid.NewGuid());
        }

        public void FixCrcs()
        {
            Header.PartitionEntryArrayCrc32 = ComputePartitionEntryCrc();
            Header.FixHeaderCrc(BytesPerSector);
        }

        public void ValidateCrcs()
        {
            ValidatePartitionEntryCrc();
            Header.ValidateHeaderCrc(BytesPerSector);
        }

        public GuidPartitionTableEntry GetEntry(string partitionName)
        {
            var partitionTableEntry = (GuidPartitionTableEntry) null;
            for (var index = 0; index < Entries.Count; ++index)
                if (string.Compare(Entries[index].PartitionName, partitionName, true, CultureInfo.InvariantCulture) ==
                    0)
                {
                    partitionTableEntry = Entries[index];
                    break;
                }

            return partitionTableEntry;
        }

        public static bool IsGuidPartitionStyle(List<DataBlockEntry> blockEntries, int bytesPerSector,
            int bytesPerBlock)
        {
            var num = bytesPerBlock / bytesPerSector;
            var dataBlockEntry = (DataBlockEntry) null;
            foreach (var blockEntry in blockEntries)
                for (var index = 0; index < blockEntry.BlockLocationsOnDisk.Count; ++index)
                    if (blockEntry.BlockLocationsOnDisk[index].AccessMethod ==
                        DiskLocation.DiskAccessMethod.DiskBegin &&
                        blockEntry.BlockLocationsOnDisk[index].BlockIndex == 0U)
                    {
                        dataBlockEntry = blockEntry;
                        break;
                    }

            return false;
        }
    }
}