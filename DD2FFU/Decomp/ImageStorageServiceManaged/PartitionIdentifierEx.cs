// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.PartitionIdentifierEx
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class PartitionIdentifierEx : BaseIdentifier, IDeviceIdentifier
    {
        private byte[] _rawData;


        public uint ElToritoValue
        {
            get => (uint)((_rawData[3] << 24) | (_rawData[2] << 16) | (_rawData[1] << 8)) | _rawData[0];
            set
            {
                _rawData[3] = (byte)((value & 4278190080U) >> 24);
                _rawData[2] = (byte)((value & 16711680U) >> 16);
                _rawData[1] = (byte)((value & 65280U) >> 8);
                _rawData[0] = (byte)(value & byte.MaxValue);
                for (int index = 4; index < 16; ++index)
                {
                    _rawData[index] = 0;
                }
            }
        }

        public Guid GptValue
        {
            get => new(_rawData);
            set => _rawData = value.ToByteArray();
        }


        public ulong MbrPartitionOffset
        {
            get => (ulong)((_rawData[7] << 24) | (_rawData[6] << 16) | (_rawData[5] << 8) | _rawData[4] |
                            (_rawData[3] << 24) | (_rawData[2] << 16) | (_rawData[1] << 8) | _rawData[0]);
            set
            {
                _rawData[7] = (byte)((value & 4278190080UL) >> 56);
                _rawData[6] = (byte)((value & 16711680UL) >> 48);
                _rawData[5] = (byte)((value & 65280UL) >> 40);
                _rawData[4] = (byte)((value & byte.MaxValue) >> 32);
                _rawData[3] = (byte)((value & 4278190080UL) >> 24);
                _rawData[2] = (byte)((value & 16711680UL) >> 16);
                _rawData[1] = (byte)((value & 65280UL) >> 8);
                _rawData[0] = (byte)(value & byte.MaxValue);
                for (int index = 8; index < 16; ++index)
                {
                    _rawData[index] = 0;
                }
            }
        }

        public IBlockIoIdentifier ParentIdentifier
        {
            get; set;
        }

        public void ReadFromStream(BinaryReader reader)
        {
            _rawData = reader.ReadBytes(16);
            ParentIdentifier = BlockIoIdentifierFactory.CreateFromStream(reader);
            ParentIdentifier.ReadFromStream(reader);
        }

        public void WriteToStream(BinaryWriter writer)
        {
            long position = writer.BaseStream.Position;
            writer.Write(_rawData);
            ParentIdentifier.WriteToStream(writer);
            while (writer.BaseStream.Position < position + Size)
            {
                writer.Write((byte)0);
            }
        }


        public void LogInfo(IULogger logger, int indentLevel)
        {
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Identifier: PartitionEx");
            if (ParentIdentifier != null)
            {
                if (ParentIdentifier.BlockType is BlockIoType.HardDisk or
                    BlockIoType.VirtualHardDisk)
                {
                    switch ((ParentIdentifier as HardDiskIdentifier ??
                                             (ParentIdentifier as VirtualDiskIdentifier).InternalIdentifer).PartitionStyle)
                    {
                        case PartitionFormat.Gpt:
                            logger.LogInfo(str + "GPT Partition Identifier: {{{0}}}", GptValue);
                            break;
                        case PartitionFormat.Mbr:
                            logger.LogInfo(str + "MBR Partition Offset: 0x{0:x}", MbrPartitionOffset);
                            break;
                        case PartitionFormat.Raw:
                            throw new ImageStorageException("Cannot use a partition identifier on a RAW disk.");
                    }
                }
                else if (ParentIdentifier.BlockType == BlockIoType.CdRom)
                {
                    logger.LogInfo(str + "El Torito Value: {0}", ElToritoValue);
                }
                else
                {
                    logger.LogInfo(str + "Value: Unsure of the partition style.");
                }
            }
            else
            {
                logger.LogInfo(str + "Value: Unsure of the partition style.");
            }

            if (ParentIdentifier == null)
            {
                return;
            }

            logger.LogInfo("");
            ParentIdentifier.LogInfo(logger, checked(indentLevel + 2));
        }


        public uint Size => ParentIdentifier != null
                    ? 16U + Math.Max(ParentIdentifier.Size, BlockIoIdentifierFactory.SizeOnDisk)
                    : 16U + BlockIoIdentifierFactory.SizeOnDisk;


        public static PartitionIdentifierEx CreateSimpleMbr(ulong partitionOffset, uint diskSignature)
        {
            return new PartitionIdentifierEx
            {
                _rawData = new byte[16],
                MbrPartitionOffset = partitionOffset,
                ParentIdentifier = HardDiskIdentifier.CreateSimpleMbr(diskSignature)
            };
        }


        public static PartitionIdentifierEx CreateSimpleGpt(Guid diskId, Guid partitionId)
        {
            return new PartitionIdentifierEx
            {
                _rawData = new byte[16],
                GptValue = partitionId,
                ParentIdentifier = HardDiskIdentifier.CreateSimpleGpt(diskId)
            };
        }
    }
}