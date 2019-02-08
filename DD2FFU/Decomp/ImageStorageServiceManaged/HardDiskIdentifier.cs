// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.HardDiskIdentifier
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class HardDiskIdentifier : BaseIdentifier, IBlockIoIdentifier, IDeviceIdentifier
    {
        private byte[] _rawIdentifier;

         public PartitionFormat PartitionStyle { get; set; }

        
        public uint MbrSignature
        {
            get => (uint) ((_rawIdentifier[3] << 24) | (_rawIdentifier[2] << 16) | (_rawIdentifier[1] << 8)) |
                   _rawIdentifier[0];
            set
            {
                _rawIdentifier[3] = (byte) ((value & 4278190080U) >> 24);
                _rawIdentifier[2] = (byte) ((value & 16711680U) >> 16);
                _rawIdentifier[1] = (byte) ((value & 65280U) >> 8);
                _rawIdentifier[0] = (byte) (value & byte.MaxValue);
                for (var index = 4; index < 16; ++index)
                    _rawIdentifier[index] = 0;
            }
        }

        public Guid GptSignature
        {
            get => new Guid(_rawIdentifier);
            set => _rawIdentifier = value.ToByteArray();
        }

        
        public uint RawDiskNumber
        {
            get => (uint) ((_rawIdentifier[3] << 24) | (_rawIdentifier[2] << 16) | (_rawIdentifier[1] << 8)) |
                   _rawIdentifier[0];
            set
            {
                _rawIdentifier[3] = (byte) ((value & 4278190080U) >> 24);
                _rawIdentifier[2] = (byte) ((value & 16711680U) >> 16);
                _rawIdentifier[1] = (byte) ((value & 65280U) >> 8);
                _rawIdentifier[0] = (byte) (value & byte.MaxValue);
                for (var index = 4; index < 16; ++index)
                    _rawIdentifier[index] = 0;
            }
        }

        internal bool AsVirtualDisk { get; set; }

        public void ReadFromStream(BinaryReader reader)
        {
            PartitionStyle = (PartitionFormat) reader.ReadUInt32();
            _rawIdentifier = reader.ReadBytes(16);
        }

        public void WriteToStream(BinaryWriter writer)
        {
            writer.Write((uint) BlockType);
            writer.Write((uint) PartitionStyle);
            writer.Write(_rawIdentifier);
        }

        
        public void LogInfo(IULogger logger, int indentLevel)
        {
            var str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Identifier: Hard Disk");
            logger.LogInfo(str + string.Format("Partition Style:   {0}", PartitionStyle));
            switch (PartitionStyle)
            {
                case PartitionFormat.Gpt:
                    logger.LogInfo(str + "GPT Guid:          {{{0}}}", (object) GptSignature);
                    break;
                case PartitionFormat.Mbr:
                    logger.LogInfo(str + string.Format("MBR Signature:     0x{0:x}", MbrSignature));
                    break;
                case PartitionFormat.Raw:
                    logger.LogInfo(str + string.Format("Raw Disk Number:   {0}", RawDiskNumber));
                    break;
            }
        }

         public uint Size => 24;

         public BlockIoType BlockType => BlockIoType.HardDisk;

        
        public static HardDiskIdentifier CreateSimpleMbr(uint diskSignature)
        {
            return new HardDiskIdentifier
            {
                PartitionStyle = PartitionFormat.Mbr,
                _rawIdentifier = new byte[16],
                MbrSignature = diskSignature
            };
        }

        
        public static HardDiskIdentifier CreateSimpleGpt(Guid diskId)
        {
            return new HardDiskIdentifier
            {
                PartitionStyle = PartitionFormat.Gpt,
                _rawIdentifier = new byte[16],
                GptSignature = diskId
            };
        }
    }
}