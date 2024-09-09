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
            Phase1DataEntries = [];
            Phase2DataEntries = [];
            Phase3DataEntries = [];
        }

        public ImageStoreHeader StoreHeader
        {
            get; set;
        }

        public List<ValidationEntry> ValidationEntries
        {
            get; set;
        }

        public List<DataBlockEntry> Phase1DataEntries
        {
            get; set;
        }

        public List<DataBlockEntry> Phase2DataEntries
        {
            get; set;
        }

        public List<DataBlockEntry> Phase3DataEntries
        {
            get; set;
        }

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
            int num = 0;
            BlockPhase phase = BlockPhase.Phase1;
            for (BlockPhase blockPhase = _recovery ? BlockPhase.Phase3 : BlockPhase.Invalid; phase != blockPhase; ++phase)
            {
                foreach (DataBlockEntry phaseEntry in GetPhaseEntries(phase))
                {
                    num += phaseEntry.SizeInBytes;
                }
            }

            return num;
        }

        public int GetMetadataSize()
        {
            return StoreHeader.GetStructureSize() + GetDescriptorSize();
        }

        public byte[] GetMetadata(uint alignment)
        {
            MemoryStream memoryStream1 = new();
            if (StoreHeader.StoreDataSizeInBytes == 0U)
            {
                StoreHeader.StoreDataSizeInBytes = (uint)GetDescriptorSize();
            }

            StoreHeader.WriteToStream(memoryStream1);
            BlockPhase phase = BlockPhase.Phase1;
            for (BlockPhase blockPhase = _recovery ? BlockPhase.Phase3 : BlockPhase.Invalid; phase != blockPhase; ++phase)
            {
                foreach (DataBlockEntry phaseEntry in GetPhaseEntries(phase))
                {
                    phaseEntry.WriteEntryToStream(memoryStream1);
                }
            }

            long num = memoryStream1.Length % alignment;
            if (num != 0L)
            {
                MemoryStream memoryStream2 = memoryStream1;
                memoryStream2.SetLength(memoryStream2.Length + alignment - num);
            }

            return memoryStream1.ToArray();
        }

        public void ReadMetadataFromStream(Stream sourceStream)
        {
            StoreHeader = ImageStoreHeader.ReadFromStream(sourceStream);
            uint num1 = StoreHeader.InitialPartitionTableBlockIndex + StoreHeader.InitialPartitionTableBlockCount;
            uint num2 = StoreHeader.FlashOnlyPartitionTableBlockIndex + StoreHeader.FlashOnlyPartitionTableBlockCount;
            uint num3 = StoreHeader.FinalPartitionTableBlockIndex + StoreHeader.FinalPartitionTableBlockCount;
            BinaryReader reader = new(sourceStream);
            uint index;
            for (index = 0U; index < num1; ++index)
            {
                DataBlockEntry dataBlockEntry = new(StoreHeader.BytesPerBlock);
                dataBlockEntry.ReadEntryFromStream(reader, index);
                Phase1DataEntries.Add(dataBlockEntry);
            }

            for (; index < num2; ++index)
            {
                DataBlockEntry dataBlockEntry = new(StoreHeader.BytesPerBlock);
                dataBlockEntry.ReadEntryFromStream(reader, index);
                Phase2DataEntries.Add(dataBlockEntry);
            }

            for (; index < num3; ++index)
            {
                DataBlockEntry dataBlockEntry = new(StoreHeader.BytesPerBlock);
                dataBlockEntry.ReadEntryFromStream(reader, index);
                Phase3DataEntries.Add(dataBlockEntry);
            }
        }

        public void LogInfo(IULogger logger, bool logStoreHeader, bool logDataEntries)
        {
            if (logStoreHeader)
            {
                StoreHeader.LogInfo(logger);
            }

            if (!logDataEntries)
            {
                return;
            }

            BlockPhase phase = BlockPhase.Phase1;
            for (BlockPhase blockPhase = _recovery ? BlockPhase.Phase3 : BlockPhase.Invalid; phase != blockPhase; ++phase)
            {
                logger.LogInfo("  {0} entries", phase);
                foreach (DataBlockEntry phaseEntry in GetPhaseEntries(phase))
                {
                    phaseEntry.LogInfo(logger, false, 4);
                }

                logger.LogInfo("");
            }
        }
    }
}