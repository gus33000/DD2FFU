// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.StorePayload
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.Collections.Generic;
using System.IO;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class StorePayload
    {
        public enum BlockPhase
        {
            Phase1,
            Phase2,
            Phase3,
            Invalid
        }

        private readonly bool _recovery;

        public StorePayload(bool recovery)
        {
            _recovery = recovery;
            StoreHeader = new ImageStoreHeader();
            Phase1DataEntries = new List<DataBlockEntry>();
            Phase2DataEntries = new List<DataBlockEntry>();
            Phase3DataEntries = new List<DataBlockEntry>();
        }

        public ImageStoreHeader StoreHeader { get; set; }

        public List<ValidationEntry> ValidationEntries { get; set; }

        public List<DataBlockEntry> Phase1DataEntries { get; set; }

        public List<DataBlockEntry> Phase2DataEntries { get; set; }

        public List<DataBlockEntry> Phase3DataEntries { get; set; }

        public List<DataBlockEntry> GetPhaseEntries(BlockPhase phase)
        {
            List<DataBlockEntry> dataBlockEntryList = null;
            switch (phase)
            {
                case BlockPhase.Phase1:
                    dataBlockEntryList = Phase1DataEntries;
                    break;
                case BlockPhase.Phase2:
                    dataBlockEntryList = Phase2DataEntries;
                    break;
                case BlockPhase.Phase3:
                    dataBlockEntryList = Phase3DataEntries;
                    break;
            }

            return dataBlockEntryList;
        }

        private int GetDescriptorSize()
        {
            var num = 0;
            var phase = BlockPhase.Phase1;
            for (var blockPhase = _recovery ? BlockPhase.Phase3 : BlockPhase.Invalid; phase != blockPhase; ++phase)
                foreach (var phaseEntry in GetPhaseEntries(phase))
                    num += phaseEntry.SizeInBytes;
            return num;
        }

        public int GetMetadataSize()
        {
            return StoreHeader.GetStructureSize() + GetDescriptorSize();
        }

        public byte[] GetMetadata(uint alignment)
        {
            var memoryStream1 = new MemoryStream();
            if (StoreHeader.StoreDataSizeInBytes == 0U)
                StoreHeader.StoreDataSizeInBytes = (uint) GetDescriptorSize();
            StoreHeader.WriteToStream(memoryStream1);
            var phase = BlockPhase.Phase1;
            for (var blockPhase = _recovery ? BlockPhase.Phase3 : BlockPhase.Invalid; phase != blockPhase; ++phase)
                foreach (var phaseEntry in GetPhaseEntries(phase))
                    phaseEntry.WriteEntryToStream(memoryStream1);
            var num = memoryStream1.Length % alignment;
            if (num != 0L)
            {
                var memoryStream2 = memoryStream1;
                memoryStream2.SetLength(memoryStream2.Length + alignment - num);
            }

            return memoryStream1.ToArray();
        }

        public void ReadMetadataFromStream(Stream sourceStream)
        {
            StoreHeader = ImageStoreHeader.ReadFromStream(sourceStream);
            var num1 = StoreHeader.InitialPartitionTableBlockIndex + StoreHeader.InitialPartitionTableBlockCount;
            var num2 = StoreHeader.FlashOnlyPartitionTableBlockIndex + StoreHeader.FlashOnlyPartitionTableBlockCount;
            var num3 = StoreHeader.FinalPartitionTableBlockIndex + StoreHeader.FinalPartitionTableBlockCount;
            var reader = new BinaryReader(sourceStream);
            uint index;
            for (index = 0U; index < num1; ++index)
            {
                var dataBlockEntry = new DataBlockEntry(StoreHeader.BytesPerBlock);
                dataBlockEntry.ReadEntryFromStream(reader, index);
                Phase1DataEntries.Add(dataBlockEntry);
            }

            for (; index < num2; ++index)
            {
                var dataBlockEntry = new DataBlockEntry(StoreHeader.BytesPerBlock);
                dataBlockEntry.ReadEntryFromStream(reader, index);
                Phase2DataEntries.Add(dataBlockEntry);
            }

            for (; index < num3; ++index)
            {
                var dataBlockEntry = new DataBlockEntry(StoreHeader.BytesPerBlock);
                dataBlockEntry.ReadEntryFromStream(reader, index);
                Phase3DataEntries.Add(dataBlockEntry);
            }
        }

        public void LogInfo(IULogger logger, bool logStoreHeader, bool logDataEntries)
        {
            if (logStoreHeader)
                StoreHeader.LogInfo(logger);
            if (!logDataEntries)
                return;
            var phase = BlockPhase.Phase1;
            for (var blockPhase = _recovery ? BlockPhase.Phase3 : BlockPhase.Invalid; phase != blockPhase; ++phase)
            {
                logger.LogInfo("  {0} entries", (object) phase);
                foreach (var phaseEntry in GetPhaseEntries(phase))
                    phaseEntry.LogInfo(logger, false, 4);
                logger.LogInfo("");
            }
        }
    }
}