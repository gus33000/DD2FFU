// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.MasterBootRecord
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class MasterBootRecord
    {
        public enum MbrParseType
        {
            Normal,
            TruncateAllExtendedRecords,
            TruncateInvalidExtendedRecords
        }

        public const ushort Signature = 43605;
        public const int FirstEntryByteOffset = 446;
        public const ushort CodeAreaSize = 440;

        private readonly byte[] CodeData = new byte[440]
        {
            51,
            192,
            142,
            208,
            188,
            0,
            124,
            142,
            192,
            142,
            216,
            190,
            0,
            124,
            191,
            0,
            6,
            185,
            0,
            2,
            252,
            243,
            164,
            80,
            104,
            28,
            6,
            203,
            251,
            185,
            4,
            0,
            189,
            190,
            7,
            128,
            126,
            0,
            0,
            124,
            11,
            15,
            133,
            14,
            1,
            131,
            197,
            16,
            226,
            241,
            205,
            24,
            136,
            86,
            0,
            85,
            198,
            70,
            17,
            5,
            198,
            70,
            16,
            0,
            180,
            65,
            187,
            170,
            85,
            205,
            19,
            93,
            114,
            15,
            129,
            251,
            85,
            170,
            117,
            9,
            247,
            193,
            1,
            0,
            116,
            3,
            254,
            70,
            16,
            102,
            96,
            128,
            126,
            16,
            0,
            116,
            38,
            102,
            104,
            0,
            0,
            0,
            0,
            102,
            byte.MaxValue,
            118,
            8,
            104,
            0,
            0,
            104,
            0,
            124,
            104,
            1,
            0,
            104,
            16,
            0,
            180,
            66,
            138,
            86,
            0,
            139,
            244,
            205,
            19,
            159,
            131,
            196,
            16,
            158,
            235,
            20,
            184,
            1,
            2,
            187,
            0,
            124,
            138,
            86,
            0,
            138,
            118,
            1,
            138,
            78,
            2,
            138,
            110,
            3,
            205,
            19,
            102,
            97,
            115,
            28,
            254,
            78,
            17,
            117,
            12,
            128,
            126,
            0,
            128,
            15,
            132,
            138,
            0,
            178,
            128,
            235,
            132,
            85,
            50,
            228,
            138,
            86,
            0,
            205,
            19,
            93,
            235,
            158,
            129,
            62,
            254,
            125,
            85,
            170,
            117,
            110,
            byte.MaxValue,
            118,
            0,
            232,
            141,
            0,
            117,
            23,
            250,
            176,
            209,
            230,
            100,
            232,
            131,
            0,
            176,
            223,
            230,
            96,
            232,
            124,
            0,
            176,
            byte.MaxValue,
            230,
            100,
            232,
            117,
            0,
            251,
            184,
            0,
            187,
            205,
            26,
            102,
            35,
            192,
            117,
            59,
            102,
            129,
            251,
            84,
            67,
            80,
            65,
            117,
            50,
            129,
            249,
            2,
            1,
            114,
            44,
            102,
            104,
            7,
            187,
            0,
            0,
            102,
            104,
            0,
            2,
            0,
            0,
            102,
            104,
            8,
            0,
            0,
            0,
            102,
            83,
            102,
            83,
            102,
            85,
            102,
            104,
            0,
            0,
            0,
            0,
            102,
            104,
            0,
            124,
            0,
            0,
            102,
            97,
            104,
            0,
            0,
            7,
            205,
            26,
            90,
            50,
            246,
            234,
            0,
            124,
            0,
            0,
            205,
            24,
            160,
            183,
            7,
            235,
            8,
            160,
            182,
            7,
            235,
            3,
            160,
            181,
            7,
            50,
            228,
            5,
            0,
            7,
            139,
            240,
            172,
            60,
            0,
            116,
            9,
            187,
            7,
            0,
            180,
            14,
            205,
            16,
            235,
            242,
            244,
            235,
            253,
            43,
            201,
            228,
            100,
            235,
            0,
            36,
            2,
            224,
            248,
            36,
            2,
            195,
            73,
            110,
            118,
            97,
            108,
            105,
            100,
            32,
            112,
            97,
            114,
            116,
            105,
            116,
            105,
            111,
            110,
            32,
            116,
            97,
            98,
            108,
            101,
            0,
            69,
            114,
            114,
            111,
            114,
            32,
            108,
            111,
            97,
            100,
            105,
            110,
            103,
            32,
            111,
            112,
            101,
            114,
            97,
            116,
            105,
            110,
            103,
            32,
            115,
            121,
            115,
            116,
            101,
            109,
            0,
            77,
            105,
            115,
            115,
            105,
            110,
            103,
            32,
            111,
            112,
            101,
            114,
            97,
            116,
            105,
            110,
            103,
            32,
            115,
            121,
            115,
            116,
            101,
            109,
            0,
            0,
            0,
            99,
            123,
            154
        };

        private readonly int _bytesPerSector;
        private readonly byte[] _codeData = new byte[440];
        private MbrPartitionEntry _extendedEntry;
        private readonly IULogger _logger;
        private MasterBootRecordMetadataPartition _metadataPartition;
        private readonly MasterBootRecord _primaryRecord;
        private uint _sectorIndex;

        private MasterBootRecord()
        {
        }

        private MasterBootRecord(MasterBootRecord primaryRecord)
        {
            _primaryRecord = primaryRecord;
            _bytesPerSector = _primaryRecord._bytesPerSector;
            _logger = _primaryRecord._logger;
        }

        public MasterBootRecord(IULogger logger, int bytesPerSector)
        {
            _logger = logger;
            _bytesPerSector = bytesPerSector;
            _metadataPartition = new MasterBootRecordMetadataPartition(logger);
        }

        public uint DiskSignature { get; set; }

        public uint DiskSectorCount { get; set; }

        public List<MbrPartitionEntry> PartitionEntries { get; } = new List<MbrPartitionEntry>();

        public MasterBootRecord ExtendedRecord { get; private set; }

        public bool ReadFromStream(Stream stream, MbrParseType parseType)
        {
            var reader = new BinaryReader(stream);
            var flag = true;
            _sectorIndex = (uint) ((ulong) stream.Position / (ulong) _bytesPerSector);
            stream.Read(_codeData, 0, _codeData.Length);
            DiskSignature = reader.ReadUInt32();
            stream.Position += 2L;
            for (var index = 0; index < 4; ++index)
            {
                var mbrPartitionEntry = new MbrPartitionEntry();
                mbrPartitionEntry.ReadFromStream(reader);
                if (IsExtendedBootRecord())
                    mbrPartitionEntry.StartingSectorOffset = _sectorIndex;
                if (mbrPartitionEntry.TypeIsContainer && parseType == MbrParseType.TruncateAllExtendedRecords)
                    mbrPartitionEntry.ZeroData();
                if (mbrPartitionEntry.TypeIsContainer)
                {
                    if (_extendedEntry != null)
                    {
                        _logger.LogWarning(
                            "{0}: The extended boot record at sector 0x{1:x} contains multiple extended boot records.",
                            (object) MethodBase.GetCurrentMethod().Name, (object) _sectorIndex);
                        if (!IsExtendedBootRecord() || parseType != MbrParseType.TruncateInvalidExtendedRecords)
                            throw new ImageStorageException("There are multiple extended partition entries.");
                        flag = false;
                        break;
                    }

                    var num = IsExtendedBootRecord()
                        ? (mbrPartitionEntry.StartingSector + _primaryRecord._extendedEntry.StartingSector) *
                          _bytesPerSector
                        : mbrPartitionEntry.StartingSector * _bytesPerSector;
                    if (mbrPartitionEntry.SectorCount == 0U || mbrPartitionEntry.StartingSector == 0U)
                    {
                        _logger.LogWarning(
                            "{0}: The boot record at sector 0x{1:x} has an entry with a extended partition type, but the start sector or size is 0.",
                            (object) MethodBase.GetCurrentMethod().Name, (object) _sectorIndex);
                        mbrPartitionEntry.PartitionType = 0;
                    }
                    else if (num > stream.Length)
                    {
                        if (parseType != MbrParseType.TruncateInvalidExtendedRecords)
                            throw new ImageStorageException("There are multiple extended partition entries.");
                        _logger.LogDebug(
                            "{0}: The extended boot entry at sector 0x{1:x} points beyond the end of the stream.",
                            (object) MethodBase.GetCurrentMethod().Name, (object) _sectorIndex);
                        if (IsExtendedBootRecord())
                        {
                            flag = false;
                            break;
                        }

                        mbrPartitionEntry.ZeroData();
                    }
                    else
                    {
                        _extendedEntry = mbrPartitionEntry;
                    }
                }

                PartitionEntries.Add(mbrPartitionEntry);
            }

            if (reader.ReadUInt16() != 43605)
            {
                if (!IsExtendedBootRecord() || parseType != MbrParseType.TruncateInvalidExtendedRecords)
                    throw new ImageStorageException("The MBR disk signature is invalid.");
                _logger.LogDebug("{0}: The extended boot record at sector 0x{1:x} has an invalid MBR signature.",
                    (object) MethodBase.GetCurrentMethod().Name, (object) _sectorIndex);
                flag = false;
            }

            if (stream.Position % _bytesPerSector != 0L)
                stream.Position += _bytesPerSector - stream.Position % _bytesPerSector;
            if (flag && !ReadExtendedPartitions(stream, parseType))
            {
                ExtendedRecord = null;
                _extendedEntry.ZeroData();
            }

            ReadMetadataPartition(stream);
            return flag;
        }

        private bool ReadExtendedPartitions(Stream stream, MbrParseType parseType)
        {
            var position = stream.Position;
            var flag = true;
            if (_extendedEntry != null)
            {
                stream.Position = IsExtendedBootRecord()
                    ? (_extendedEntry.StartingSector + _primaryRecord._extendedEntry.StartingSector) * _bytesPerSector
                    : _extendedEntry.StartingSector * _bytesPerSector;
                ExtendedRecord = new MasterBootRecord(_primaryRecord == null ? this : _primaryRecord);
                flag = ExtendedRecord.ReadFromStream(stream, parseType);
            }

            return flag;
        }

        public void WriteToStream(Stream stream, bool addCodeData)
        {
            var writer = new BinaryWriter(stream);
            stream.Position = _sectorIndex * _bytesPerSector;
            if (!addCodeData)
                stream.Write(_codeData, 0, 440);
            else
                stream.Write(CodeData, 0, CodeData.Length);
            writer.Write(DiskSignature);
            stream.WriteByte(0);
            stream.WriteByte(0);
            foreach (var entry in PartitionEntries)
                entry.WriteToStream(writer);
            writer.Write((ushort) 43605);
            if (stream.Position % _bytesPerSector != 0L)
                stream.Position += _bytesPerSector - stream.Position % _bytesPerSector;
            if (ExtendedRecord == null)
                return;
            ExtendedRecord.WriteToStream(stream, false);
        }

        public void LogInfo(IULogger logger, ushort indentLevel = 0)
        {
            var str = new StringBuilder().Append(' ', indentLevel).ToString();
            if (IsValidProtectiveMbr())
            {
                logger.LogInfo(str + "Protective Master Boot Record");
            }
            else if (_primaryRecord == null)
            {
                logger.LogInfo(str + "Master Boot Record");
                logger.LogInfo(str + "  Disk Signature: 0x{0:x}", (object) DiskSignature);
            }
            else
            {
                logger.LogInfo(str + "Extended Boot Record");
            }

            logger.LogInfo("");
            foreach (var entry in PartitionEntries)
                entry.LogInfo(logger, this, (ushort) (indentLevel + 2U));
            if (ExtendedRecord == null)
                return;
            if (!IsExtendedBootRecord())
                indentLevel += 2;
            ExtendedRecord.LogInfo(logger, indentLevel);
        }

        public bool IsValidProtectiveMbr()
        {
            for (var index = 0; index < PartitionEntries.Count; ++index)
            {
                var entry = PartitionEntries[index];
                if (index == 0)
                {
                    if (entry.StartingSector != 1U || entry.SectorCount == 0U || entry.PartitionType != 238)
                        return false;
                }
                else if (entry.SectorCount != 0U || entry.StartingSector != 0U || entry.PartitionType != 0)
                {
                    return false;
                }

                if (entry.Bootable)
                    return false;
            }

            return true;
        }

        public bool IsExtendedBootRecord()
        {
            return _primaryRecord != null;
        }

        public MbrPartitionEntry FindPartitionByType(byte partitionType)
        {
            foreach (var entry in PartitionEntries)
                if (entry.PartitionType == partitionType)
                    return entry;
            if (ExtendedRecord != null)
                return ExtendedRecord.FindPartitionByType(partitionType);
            return null;
        }

        public ulong FindPartitionOffset(string partitionName)
        {
            ulong num = 0;
            if (_metadataPartition != null)
                foreach (var entry in _metadataPartition.Entries)
                    if (string.Compare(entry.Name, partitionName, true, CultureInfo.InvariantCulture) == 0)
                    {
                        num = entry.DiskOffset;
                        break;
                    }

            return num;
        }

        public MbrPartitionEntry FindPartitionByName(string partitionName)
        {
            var partitionOffset = FindPartitionOffset(partitionName);
            if (partitionOffset > 0UL)
                return FindPartitionByName(partitionName, partitionOffset);
            return null;
        }

        private MbrPartitionEntry FindPartitionByName(string partitionName, ulong diskOffset)
        {
            foreach (var entry in PartitionEntries)
                if (entry.AbsoluteStartingSector * _bytesPerSector == (long) diskOffset)
                    return entry;
            if (ExtendedRecord != null)
                return ExtendedRecord.FindPartitionByName(partitionName, diskOffset);
            return null;
        }

        public string GetPartitionName(MbrPartitionEntry entry)
        {
            var num = entry.AbsoluteStartingSector * (ulong) _bytesPerSector;
            var metadataPartition = _metadataPartition;
            if (IsExtendedBootRecord())
                metadataPartition = _primaryRecord._metadataPartition;
            if (metadataPartition != null)
                foreach (var entry1 in metadataPartition.Entries)
                    if ((long) entry1.DiskOffset == (long) num)
                        return entry1.Name;
            return string.Empty;
        }

        public long GetMetadataPartitionOffset()
        {
            long num = 0;
            var partitionByType = FindPartitionByType(MasterBootRecordMetadataPartition.PartitonType);
            if (partitionByType != null)
                num = partitionByType.AbsoluteStartingSector * _bytesPerSector;
            return num;
        }

        public void ReadMetadataPartition(Stream stream)
        {
            if (_primaryRecord != null || IsValidProtectiveMbr())
                return;
            var metadataPartitionOffset = GetMetadataPartitionOffset();
            var position = stream.Position;
            if (metadataPartitionOffset > 0L)
            {
                stream.Position = metadataPartitionOffset;
                _metadataPartition = new MasterBootRecordMetadataPartition(_logger);
                _metadataPartition.ReadFromStream(stream);
            }

            stream.Position = position;
        }

        public byte[] GetCodeData()
        {
            return _codeData;
        }

        public void RemovePartition(string partitionName)
        {
            var partitionOffset = FindPartitionOffset(partitionName);
            if (partitionOffset == 0UL)
                throw new ImageStorageException(
                    string.Format("Partition {0} was not found in the MBR metadata partition.", partitionName));
            RemovePartition(partitionName, partitionOffset);
        }

        private void RemovePartition(string partitionName, ulong partitionOffset)
        {
            var flag = true;
            foreach (var entry in PartitionEntries)
                if (entry.AbsoluteStartingSector * _bytesPerSector == (long) partitionOffset)
                {
                    entry.ZeroData();
                    flag = false;
                    break;
                }

            if (!flag)
                return;
            if (ExtendedRecord == null)
                throw new ImageStorageException(string.Format(
                    "Partition {0} was in the MBR metadata partition, but the boot record is not found.",
                    partitionName));
            ExtendedRecord.RemovePartition(partitionName, partitionOffset);
        }
    }
}