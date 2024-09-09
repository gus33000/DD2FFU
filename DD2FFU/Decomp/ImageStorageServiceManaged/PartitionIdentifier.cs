// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.PartitionIdentifier
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Reflection;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class PartitionIdentifier : BaseIdentifier, IDeviceIdentifier
    {
        public uint ElToritoValue
        {
            get; set;
        }

        public Guid GptValue
        {
            get; set;
        }

        public uint MbrPartitionNumber
        {
            get; set;
        }

        public IBlockIoIdentifier ParentIdentifier
        {
            get; set;
        }

        public void ReadFromStream(BinaryReader reader)
        {
            byte[] b = reader.ReadBytes(16);
            GptValue = new Guid(b);
            ElToritoValue = (uint)((b[3] << 24) | (b[2] << 16) | (b[1] << 8)) | b[0];
            MbrPartitionNumber = (uint)((b[3] << 24) | (b[2] << 16) | (b[1] << 8)) | b[0];
            ParentIdentifier = BlockIoIdentifierFactory.CreateFromStream(reader);
            ParentIdentifier.ReadFromStream(reader);
        }

        public void WriteToStream(BinaryWriter writer)
        {
            throw new ImageStorageException(string.Format("{0}: This function isn't implemented.",
                MethodBase.GetCurrentMethod().Name));
        }


        public void LogInfo(IULogger logger, int indentLevel)
        {
            string str = new StringBuilder().Append(' ', indentLevel).ToString();
            logger.LogInfo(str + "Identifier: Partition");
            if (ParentIdentifier != null)
            {
                if (ParentIdentifier.BlockType == BlockIoType.HardDisk)
                {
                    switch ((ParentIdentifier as HardDiskIdentifier).PartitionStyle)
                    {
                        case PartitionFormat.Gpt:
                            logger.LogInfo(str + "GPT Partition Identifier: {{{0}}}", GptValue);
                            break;
                        case PartitionFormat.Mbr:
                            logger.LogInfo(str + "MBR Partition Number: {0}", MbrPartitionNumber);
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
        }

        public uint Size => 0;
    }
}