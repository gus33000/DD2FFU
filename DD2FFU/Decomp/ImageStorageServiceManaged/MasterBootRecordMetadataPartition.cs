// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.MasterBootRecordMetadataPartition
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.Collections.Generic;
using System.IO;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class MasterBootRecordMetadataPartition
    {
        public static byte PartitonType = ImageConstants.MBR_METADATA_PARTITION_TYPE;
        public static uint HeaderSignature = 524289;
        private readonly IULogger _logger;

        private MasterBootRecordMetadataPartition()
        {
        }

        public MasterBootRecordMetadataPartition(IULogger logger)
        {
            _logger = logger;
        }

        public List<MetadataPartitionEntry> Entries { get; } = new List<MetadataPartitionEntry>();

        public MetadataPartitionHeader Header { get; } = new MetadataPartitionHeader();

        public void ReadFromStream(Stream stream)
        {
            var reader = new BinaryReader(stream);
            Header.ReadFromStream(reader);
            for (uint index = 0; index < Header.PartitionCount; ++index)
            {
                var metadataPartitionEntry = new MetadataPartitionEntry();
                metadataPartitionEntry.ReadFromStream(reader);
                Entries.Add(metadataPartitionEntry);
            }
        }

        public void LogInfo(ushort indentLevel = 0)
        {
            Header.LogInfo(_logger, (ushort) (indentLevel + 2U));
            foreach (var entry in Entries)
                entry.LogInfo(_logger, (ushort) (indentLevel + 2U));
        }
    }
}